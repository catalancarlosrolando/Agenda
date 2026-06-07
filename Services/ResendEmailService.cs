using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AgendaApi.Services;

public class ResendEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ResendEmailService> _logger;

    private readonly string _apiKey;

    public ResendEmailService(HttpClient httpClient, ILogger<ResendEmailService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _apiKey = configuration["VariablesExternas:ResendApiKey"] ?? "API_KEY_POR_DEFECTO";
    }

    public async Task EnviarAlertaAsync(string destino, string tituloEvento, DateTime fechaHora)
    {
        try
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // Payload estándar que exige la API de Resend
            var payload = new
            {
                from = "Agenda Inteligente <onboarding@resend.dev>",
                to = new[] { destino },
                subject = $"⏰ Recordatorio: {tituloEvento}",
                html = $"<div style='font-family: sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>" +
                       $"<h2>¡Hola! Tenés un evento próximo</h2>" +
                       $"<p>Te recordamos que tu evento <strong>{tituloEvento}</strong> está programado para el <strong>{fechaHora:dd/MM/yyyy HH:mm} hs</strong>.</p>" +
                       $"<hr/><p style='font-size: 12px; color: #777;'>Este es un correo automático generado por tu Agenda Core.</p>" +
                       $"</div>"
            };

            requestMessage.Content = JsonContent.Create(payload);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("📧 [RESEND] Correo real enviado con éxito a {Destino}.", destino);
            }
            else
            {
                var errorRes = await response.Content.ReadAsStringAsync();
                _logger.LogError("❌ [RESEND] Error al enviar mail. Código: {Code}. Detalle: {Error}", response.StatusCode, errorRes);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [RESEND] Excepción crítica al intentar conectar con la API.");
        }
    }
}