using AgendaApi.Data;
using AgendaApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Features.Eventos;

public class ObtenerEventosHandler : IRequestHandler<ObtenerEventosQuery, List<Evento>>
{
    private readonly AgendaContext _context;

    public ObtenerEventosHandler(AgendaContext context) => _context = context;

    public async Task<List<Evento>> Handle(ObtenerEventosQuery request, CancellationToken cancellationToken)
    {
        return await _context.Eventos.ToListAsync(cancellationToken);
    }
}