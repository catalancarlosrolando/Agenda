namespace AgendaApi.Services;

public class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;
    public MockEmailService(ILogger<MockEmailService> logger) => _logger = logger;

    public Task EnviarAlertaAsync(string destino, string tituloEvento, DateTime fechaHora)
    {
        _logger.LogInformation("--> [SIMULACIÓN RESEND] Mail enviado con éxito a {Destino} para el evento '{Titulo}'", destino, tituloEvento);
        return Task.CompletedTask;
    }
}

public class MockSmsService : ISmsService
{
    private readonly ILogger<MockSmsService> _logger;
    public MockSmsService(ILogger<MockSmsService> logger) => _logger = logger;

    public Task EnviarAlertaCriticaAsync(string destino, string tituloEvento, DateTime fechaHora)
    {
        _logger.LogInformation("--> [SIMULACIÓN TWILIO] SMS enviado con éxito a {Destino} para el evento '{Titulo}'", destino, tituloEvento);
        return Task.CompletedTask;
    }
}