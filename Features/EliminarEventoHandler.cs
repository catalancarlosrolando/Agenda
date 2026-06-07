using AgendaApi.Data;
using MediatR;

namespace AgendaApi.Features.Eventos;

public class EliminarEventoHandler : IRequestHandler<EliminarEventoCommand, bool>
{
    private readonly AgendaContext _context;

    public EliminarEventoHandler(AgendaContext context) => _context = context;

    public async Task<bool> Handle(EliminarEventoCommand request, CancellationToken cancellationToken)
    {
        var evento = await _context.Eventos.FindAsync(new object[] { request.Id }, cancellationToken);

        if (evento == null) return false;

        _context.Eventos.Remove(evento);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}