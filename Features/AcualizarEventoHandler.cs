using AgendaApi.Data;
using MediatR;
using AgendaApi.Services; // Para la cola de notificaciones

namespace AgendaApi.Features.Eventos;

public class ActualizarEventoHandler : IRequestHandler<ActualizarEventoCommand, bool>
{
    private readonly AgendaContext _context;

    private readonly INotificationQueue _queue;

    public ActualizarEventoHandler(AgendaContext context, INotificationQueue queue)
    {
        _context = context;
        _queue = queue;
    }

    public async Task<bool> Handle(ActualizarEventoCommand request, CancellationToken cancellationToken)
    {
        var evento = await _context.Eventos.FindAsync(new object[] { request.Id }, cancellationToken);

        if (evento == null) return false;

        // Actualizamos los campos
        evento.Titulo = request.Titulo;
        evento.Descripcion = request.Descripcion;
        evento.FechaHora = request.FechaHora;
        evento.Tipo = request.Tipo;
        evento.Prioridad = request.Prioridad;
        evento.EmailDestino = request.EmailDestino;
        evento.TelefonoDestino = request.TelefonoDestino;
        evento.AlertaConfirmada = request.AlertaConfirmada;

        // Si el usuario cambia la fecha o el destino, reiniciamos el flujo de envío para que el Worker vuelva a evaluar el evento
        evento.MailEnviado = false;
        evento.SmsEnviado = false;

        await _context.SaveChangesAsync(cancellationToken);

        await _queue.EscribirEventoAsync(evento.Id);

        return true;
    }
}