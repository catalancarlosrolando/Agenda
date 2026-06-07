using AgendaApi.Models; // Ajustá al namespace de tu DbContext y Evento
using AgendaApi.Data;   // Tu namespace del ApplicationDbContext
using MediatR;

namespace AgendaApi.Features.Eventos;

public class CrearEventoHandler : IRequestHandler<CrearEventoCommand, int>
{
    private readonly AgendaContext _context;

    // Inyectamos el DbContext directamente en el Handler, ya no en el controlador
    public CrearEventoHandler(AgendaContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CrearEventoCommand request, CancellationToken cancellationToken)
    {
        // Mapeamos los datos del comando a la entidad de base de datos
        var nuevoEvento = new Evento
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            FechaHora = request.FechaHora,
            Tipo = request.Tipo,
            Prioridad = request.Prioridad,
            EmailDestino = request.EmailDestino,
            TelefonoDestino = request.TelefonoDestino,
            MailEnviado = false,
            SmsEnviado = false,
            AlertaConfirmada = false
        };

        // Guardamos en la base de datos de forma asíncrona
        _context.Eventos.Add(nuevoEvento);
        await _context.SaveChangesAsync(cancellationToken);

        // Devolvemos el ID generado para que el controlador arme la respuesta HTTP
        return nuevoEvento.Id;
    }
}