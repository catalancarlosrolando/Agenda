using MediatR;

namespace AgendaApi.Features.Eventos;

// IRequest<int> significa que este comando, al procesarse, va a devolver el ID del evento creado
public record CrearEventoCommand(
    string Titulo,
    string? Descripcion,
    DateTime FechaHora,
    string? Tipo,
    int Prioridad,
    string EmailDestino,
    string TelefonoDestino
) : IRequest<int>;