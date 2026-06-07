using AgendaApi.Data;
using AgendaApi.Models;
using MediatR;

namespace AgendaApi.Features.Eventos;

public class ObtenerEventoPorIdHandler : IRequestHandler<ObtenerEventoPorIdQuery, Evento?>
{
    private readonly AgendaContext _context;

    public ObtenerEventoPorIdHandler(AgendaContext context) => _context = context;

    public async Task<Evento?> Handle(ObtenerEventoPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Eventos.FindAsync(new object[] { request.Id }, cancellationToken);
    }
}