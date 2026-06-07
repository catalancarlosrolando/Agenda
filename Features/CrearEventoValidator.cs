using FluentValidation;
using AgendaApi.Data; // Tu namespace del DbContext
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Features.Eventos;

public class CrearEventoValidator : AbstractValidator<CrearEventoCommand>
{
    private readonly AgendaContext _context;

    public CrearEventoValidator(AgendaContext context)
    {

        _context = context;

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(100).WithMessage("El título no puede superar los 100 caracteres.")
            .MustAsync(BeUniqueTitle).WithMessage("Ya existe un evento agendado con ese mismo título."); // 👈 NUEVA REGLA ASÍNCRONA

        RuleFor(x => x.FechaHora)
            .NotEmpty().WithMessage("La fecha y hora son obligatorias.")
            .GreaterThan(DateTime.Now).WithMessage("No podés agendar un evento en el pasado.");

        RuleFor(x => x.Prioridad)
            .InclusiveBetween(1, 3).WithMessage("La prioridad debe ser entre 1 (Baja) y 3 (Alta).");

        RuleFor(x => x.EmailDestino)
            .NotEmpty().WithMessage("El email es obligatorio para las notificaciones.")
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

        RuleFor(x => x.TelefonoDestino)
            .NotEmpty().WithMessage("El teléfono es obligatorio para alertas críticas por SMS.")
            .Matches(@"^\+[1-9]\d{1,14}$").WithMessage("El teléfono debe estar en formato internacional (Ej: +549264XXXXXXX).");
    }

    // Este método va a la DB y devuelve true si el título NO existe(es válido)
    private async Task<bool> BeUniqueTitle(string titulo, CancellationToken cancellationToken)
    {
        var existe = await _context.Eventos
            .AnyAsync(e => e.Titulo.ToLower() == titulo.ToLower(), cancellationToken);

        return !existe;
    }
}