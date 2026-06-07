using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AgendaApi.Services;

public class TwilioSmsService : ISmsService
{
    private readonly ILogger<TwilioSmsService> _logger;

    public readonly string _accountSid;
    public readonly string _authToken;
    public readonly string DesdeTelefonoTwilio;


    public TwilioSmsService(ILogger<TwilioSmsService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _accountSid = configuration["VariablesExternas:TwilioAccountSid"] ?? "TWILIO_SID_POR_DEFECTO";
        _authToken = configuration["VariablesExternas:TwilioAuthToken"] ?? "TWILIO_AUTH_POR_DEFECTO";
        DesdeTelefonoTwilio = configuration["VariablesExternas:TwilioNumero"] ?? "+10000000000";

        // Inicializamos el cliente global de Twilio con tus credenciales
        TwilioClient.Init(_accountSid, _authToken);
    }

    public async Task EnviarAlertaCriticaAsync(string destino, string tituloEvento, DateTime fechaHora)
    {
        try
        {
            // Disparamos el SMS de forma asíncrona a través de la librería de Twilio
            var mensaje = await MessageResource.CreateAsync(
                to: new PhoneNumber(destino), // Tu celular verificado (Ej: +549264XXXXXXX)
                from: new PhoneNumber(DesdeTelefonoTwilio),
                body: $"🚨 ALERTA CRÍTICA: Tu evento '{tituloEvento}' comienza pronto ({fechaHora:HH:mm} hs). Ingresá a la app si necesitas reprogramarlo."
            );

            _logger.LogInformation("🚨 [TWILIO] SMS real enviado con éxito. SID: {Sid}", mensaje.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [TWILIO] Error crítico al intentar enviar el SMS.");
        }
    }
}