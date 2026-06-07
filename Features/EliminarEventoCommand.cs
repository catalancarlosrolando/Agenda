using MediatR;

namespace AgendaApi.Features.Eventos;

public record EliminarEventoCommand(int Id) : IRequest<bool>;