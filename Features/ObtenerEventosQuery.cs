using AgendaApi.Models;
using MediatR;

namespace AgendaApi.Features.Eventos;

public record ObtenerEventosQuery() : IRequest<List<Evento>>;

public record ObtenerEventoPorIdQuery(int Id) : IRequest<Evento?>;