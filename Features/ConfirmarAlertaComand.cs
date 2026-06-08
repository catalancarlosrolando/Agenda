using MediatR;

// Solo viaja el ID y el estado nuevo
public record ConfirmarAlertaCommand(int Id, bool AlertaConfirmada) : IRequest<bool>;