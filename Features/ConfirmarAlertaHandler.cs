using MediatR;
using AgendaApi.Data;

namespace AgendaApi.Features.Eventos;


public class ConfirmarAlertaHandler : IRequestHandler<ConfirmarAlertaCommand, bool>
{
    private readonly AgendaContext _context;

    public ConfirmarAlertaHandler(AgendaContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ConfirmarAlertaCommand request, CancellationToken cancellationToken)
    {
        var evento = await _context.Eventos.FindAsync(request.Id);

        if (evento == null)
            return false; // No se encontró el evento

        evento.AlertaConfirmada = request.AlertaConfirmada;
        await _context.SaveChangesAsync(cancellationToken);

        return true; // Actualización exitosa
    }
}