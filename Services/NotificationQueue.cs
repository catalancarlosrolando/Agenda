using System.Threading.Channels;

namespace AgendaApi.Services;

public interface INotificationQueue
{
    ValueTask EscribirEventoAsync(int eventoId);
    ValueTask<int> LeerEventoAsync(CancellationToken cancellationToken);
}

public class NotificationQueue : INotificationQueue
{
    // Creamos un canal seguro para hilos que transporta enteros (los IDs de los eventos)
    private readonly Channel<int> _queue = Channel.CreateUnbounded<int>(new UnboundedChannelOptions
    {
        SingleReader = true, // Solo nuestro BackgroundService va a leer la cola
        SingleWriter = false // Múltiples handlers pueden escribir al mismo tiempo (concurrencia)
    });

    public async ValueTask EscribirEventoAsync(int eventoId)
    {
        await _queue.Writer.WriteAsync(eventoId);
    }

    public async ValueTask<int> LeerEventoAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}