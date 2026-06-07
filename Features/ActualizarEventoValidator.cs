using FluentValidation;
using AgendaApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Features.Eventos;

public class ActualizarEventoValidator : AbstractValidator<ActualizarEventoCommand>
{
    private readonly AgendaContext _context;

    public ActualizarEventoValidator(AgendaContext context)
    {

        _context = context;

        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Titulo)
        .NotEmpty().WithMessage("El título es obligatorio.")
        .MaximumLength(100)
        .MustAsync(async (comando, titulo, cancellationToken) =>
                await BeUniqueTitleForUpdate(comando.Id, titulo, cancellationToken))
            .WithMessage("Ya existe otro evento agendado con ese mismo título.");

        RuleFor(x => x.FechaHora).NotEmpty().WithMessage("La fecha y hora son obligatorias."); // Nota: En una actualización, tal vez permitas fechas más cercanas o pasadas si estás editando una nota histórica, vos decidís si le ponés el GreaterThan(DateTime.Now).
        RuleFor(x => x.Prioridad).InclusiveBetween(1, 3);
        RuleFor(x => x.EmailDestino).NotEmpty().EmailAddress();
        RuleFor(x => x.TelefonoDestino).NotEmpty().Matches(@"^\+[1-9]\d{1,14}$");
    }

    private async Task<bool> BeUniqueTitleForUpdate(int id, string titulo, CancellationToken cancellationToken)
    {
        // Buscamos si existe algún evento con ese título, pero que tenga un ID DIFERENTE al que estamos editando
        var existeOtro = await _context.Eventos
            .AnyAsync(e => e.Id != id && e.Titulo.ToLower() == titulo.ToLower(), cancellationToken);

        return !existeOtro;
    }
}