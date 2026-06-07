using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AgendaApi.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ocurrió una excepción no controlada: {Message}", exception.Message);

        // Si la excepción es de nuestro tubo de validación, armamos una respuesta limpia
        if (exception is ValidationException validationException)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            // Agrupamos los errores por el nombre del campo que falló
            var erroresPorCampo = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "Errores de validación de negocio.",
                Detail = "Un o más campos no cumplen con las reglas del sistema.",
                Instance = httpContext.Request.Path
            };

            // Metemos nuestros mensajes en español dentro del formato estándar de errores de .NET
            problemDetails.Extensions.Add("errors", erroresPorCampo);

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true; // Le avisamos a .NET que ya manejamos el error
        }

        // Si es cualquier otro tipo de error (ej. se cayó la DB), dejamos que devuelva un 500 genérico seguro
        return false;
    }
}