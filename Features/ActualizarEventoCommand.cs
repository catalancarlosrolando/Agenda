using MediatR;

namespace AgendaApi.Features.Eventos;

public record ActualizarEventoCommand(
    int Id,
    string Titulo,
    string? Descripcion,
    DateTime FechaHora,
    string? Tipo,
    int Prioridad,
    string EmailDestino,
    string TelefonoDestino,
    bool AlertaConfirmada // Por si quieren desactivar el SMS desde la UI
) : IRequest<bool>; // Devuelve true si existía y se actualizó, false si no se encontró