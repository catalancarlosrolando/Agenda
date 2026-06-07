using AgendaApi.Data;
using MediatR;

namespace AgendaApi.Features.Eventos;

public class ActualizarEventoHandler : IRequestHandler<ActualizarEventoCommand, bool>
{
    private readonly AgendaContext _context;

    public ActualizarEventoHandler(AgendaContext context) => _context = context;

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
        return true;
    }
}