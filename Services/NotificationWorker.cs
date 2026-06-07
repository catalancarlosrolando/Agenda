using AgendaApi.Data;
using AgendaApi.Services;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Workers;

public class NotificationWorker : BackgroundService
{
    private readonly INotificationQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationWorker> _logger;

    public NotificationWorker(
        INotificationQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Motor de Notificaciones Inteligentes iniciado...");

        // Hilos paralelos: Uno escucha la cola y el otro corre el reloj de fondo
        var tareaCola = EscucharColaEventosAsync(stoppingToken);
        var tareaReloj = ServidorRelojEscalamientoAsync(stoppingToken);

        await Task.WhenAll(tareaCola, tareaReloj);
    }

    // TAREA 1: Reacciona inmediatamente cuando entra algo a la cola (Canal)
    private async Task EscucharColaEventosAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Se queda esperando de forma asíncrona (sin consumir CPU) a que caiga un ID
                int eventoId = await _queue.LeerEventoAsync(stoppingToken);
                _logger.LogInformation("🔥 Evento detectado en la cola (ID: {Id}). Planificando alertas...", eventoId);

                // Acá podrías hacer comprobaciones iniciales si quisieras
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar ID de la cola.");
            }
        }
    }

    // TAREA 2: El reloj de escalamiento que revisa la DB cada 60 segundos
    private async Task ServidorRelojEscalamientoAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("⏰ Reloj de Escalamiento: Revisando eventos próximos en la base de datos...");

            try
            {
                // Creamos un Scope bajo demanda para poder usar el DbContext con seguridad
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AgendaContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var smsService = scope.ServiceProvider.GetRequiredService<ISmsService>();

                    var ahora = DateTime.Now;

                    // Buscamos eventos futuros que necesiten Mail o SMS
                    var eventosProximos = await context.Eventos
                        .Where(e => e.FechaHora > ahora && !e.AlertaConfirmada)
                        .ToListAsync(stoppingToken);

                    foreach (var ev in eventosProximos)
                    {
                        var tiempoRestante = ev.FechaHora - ahora;

                        // Escalón 1: Faltan menos de 24 horas y no se mandó el Mail
                        if (tiempoRestante.TotalHours <= 24 && !ev.MailEnviado)
                        {
                            _logger.LogWarning("📧 Disparando Escalón 1 (Mail) para el evento: {Titulo}", ev.Titulo);

                            await emailService.EnviarAlertaAsync(ev.EmailDestino, ev.Titulo, ev.FechaHora);

                            ev.MailEnviado = true;
                            context.Entry(ev).Property(x => x.MailEnviado).IsModified = true;
                        }

                        // Escalón 2: Alerta Crítica. Faltan menos de 2 horas, NO confirmó lectura y no se mandó el SMS
                        if (tiempoRestante.TotalHours <= 2 && !ev.SmsEnviado)
                        {
                            _logger.LogCritical("🚨 ESCALAMIENTO DISPARADO (SMS de emergencia) para el evento: {Titulo}", ev.Titulo);

                            await smsService.EnviarAlertaCriticaAsync(ev.TelefonoDestino, ev.Titulo, ev.FechaHora);

                            ev.SmsEnviado = true;
                            context.Entry(ev).Property(x => x.SmsEnviado).IsModified = true;
                        }
                    }

                    // Guardamos los cambios de los estados de envío de vuelta en la DB
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el ciclo del reloj de escalamiento.");
            }

            // Espera un minuto exacto de forma asíncrona antes de volver a revisar la DB
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}