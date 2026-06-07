namespace AgendaApi.Models;

public class Evento
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public DateTime FechaHora { get; set; }
    public string? Tipo { get; set; }
    public int Prioridad { get; set; } = 1; // 1 = Baja, 2 = Media, 3 = Alta

    // --- NUEVOS CAMPOS PARA EL MOTOR DE ESCALAMIENTO ---

    // El email del usuario al que se le notificará (para producción)
    public string EmailDestino { get; set; } = null!;

    // El teléfono celular para el SMS de Twilio
    public string TelefonoDestino { get; set; } = null!;

    // Control de Escalamiento paso a paso
    public bool MailEnviado { get; set; } = false;
    public bool SmsEnviado { get; set; } = false;

    // El "botón de pánico" o confirmación. 
    // Si el usuario confirma que vio la alerta (vía un endpoint), se detiene el escalamiento a SMS.
    public bool AlertaConfirmada { get; set; } = false;
}