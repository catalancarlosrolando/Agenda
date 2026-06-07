
namespace AgendaApi.Services;

public interface IEmailService
{
    Task EnviarAlertaAsync(string destino, string tituloEvento, DateTime fechaHora);
}

public interface ISmsService
{
    Task EnviarAlertaCriticaAsync(string destino, string tituloEvento, DateTime fechaHora);
}