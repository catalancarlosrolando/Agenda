.NET Core maneja la asincronía (async/await) de forma nativa a través del Thread Pool. Cuando un endpoint hace una consulta pesada a la base de datos de manera asíncrona, el hilo que atendía esa petición se libera para atender a otros usuarios, evitando el cuello de botella en el servidor web. Sin embargo, la base de datos sigue sufriendo, porque la asincronía en el backend no disminuye la cantidad de consultas que le llegan a la DB (como le pasó al creador de Coolify en el post que analizamos).


1-  creamos la webapi con el comando `dotnet new webapi -n AgendaApi` y luego agregamos los paquetes necesarios para trabajar con Entity Framework Core, MediatR y FluentValidation. Para esto, ejecutamos los siguientes comandos en la terminal:

```

2- instalamos MediatR y FluentValidation para implementar el patrón CQRS en nuestro proyecto. MediatR nos permite separar las responsabilidades de nuestras acciones (queries y commands) y FluentValidation nos ayuda a validar los datos de entrada de manera eficiente.

MediatR se instala con el siguiente comando:

```dotnet add package MediatR
```
FluentValidation se instala con el siguiente comando:

```dotnet add package FluentValidation
```

importamos los namespaces necesarios en nuestro archivo Program.cs para configurar los servicios de MediatR y FluentValidation. Esto se hace agregando las siguientes líneas al inicio del archivo:

```csharp
using FluentValidation;
using MediatR;
using AgendaApi.Behaviors;

creamos ValidationBehavior.cs para implementar un Pipeline Behavior que se encargue de validar las peticiones antes de que lleguen a los Handlers. Este middleware interceptará las peticiones y ejecutará las validaciones definidas en FluentValidation, deteniendo la ejecución si alguna validación falla y retornando un error 400 Bad Request.

ahora vamos a crear El motor de notificaciones para enviar correos electrónicos o mensajes de texto cuando se creen o actualicen eventos. Para esto, creamos una clase NotificationHandler que implementa INotificationHandler de MediatR. Esta clase se encargará de manejar las notificaciones y enviar los mensajes correspondientes.

----
Paso 1: Paso 1: Crear la Cola de Mensajes (NotificationQueue)
Vamos a encapsular un Channel<int> de .NET. Usamos un canal de tipo Unbounded (sin límite) porque para una agenda común la memoria no va a sufrir y es extremadamente eficiente.

Creá una carpeta llamada Services o Infrastructure y agregá la clase NotificationQueue.cs:

Paso 2: Registrar la Cola como Singleton
Como la cola tiene que vivir en la memoria de la aplicación durante todo el ciclo de vida del servidor, debe registrarse como un servicio Singleton en tu Program.cs.

Paso 3: Hacer que el Handler "Encole" el Evento
Ahora vamos a modificar tu CrearEventoHandler.cs. Vamos a inyectar la cola (INotificationQueue) y, justo después del SaveChangesAsync(), meteremos el ID en la cola.

```csharp
await _queue.EscribirEventoAsync(nuevoEvento.Id);
```
Tambien vamos a hacer lo mismo en el ActualizarEventoHandler.cs, para que cada vez que se actualice un evento, su ID también se encole.

Paso 4: Crear un Worker para Procesar la Cola
Finalmente, vamos a crear un Worker (BackgroundService) que se ejecute en segundo plano y procese los IDs que se encolan. Este Worker se encargará de leer los IDs de la cola y enviar las notificaciones correspondientes (por ejemplo, enviando un correo electrónico o un mensaje de texto).

Se va a encargar de dos tareas críticas de forma asíncrona:

*** a - Vaciar la cola: Reaccionar de inmediato cuando entra un ID nuevo a la cola (por si queremos registrar logs o auditorías al instante).

*** b - El Reloj de Escalamiento: Despertarse cada 60 segundos, revisar la base de datos y evaluar las ventanas de tiempo críticas (Mail a las 24hs ➔ SMS a las 2hs si no hay confirmación).

    Paso 1: Definir las Interfaces de Correo y SMS
    Para mantener el código limpio y desacoplado, vamos a definir interfaces para el servicio de correo electrónico y el servicio de SMS. Esto nos permitirá implementar diferentes estrategias de envío de notificaciones sin afectar el resto del código.

    Paso 2: Crear el Worker en Segundo Plano (NotificationWorker)
    Este es el motor principal. Como es un servicio de larga duración (Long-running service), hereda de BackgroundService.

    Un detalle técnico muy avanzado de .NET que vas a implementar acá es el manejo de Scopes. Un BackgroundService es un Singleton (vive para siempre), pero tu ApplicationDbContext es Scoped (se crea y destruye en cada petición). No podés inyectar un servicio Scoped directo en un Singleton porque se rompe. Para solucionarlo, inyectamos IServiceScopeFactory y creamos un scope a mano cada vez que necesitamos ir a la base de datos.
    
    Paso 3: Registrar el Worker en Program.cs
    Para que .NET sepa que tiene que arrancar este proceso paralelo al encender la aplicación, tenés que registrarlo como un Hosted Service.

    Paso 4: Como el worker va a pedir las implementaciones de IEmailService e ISmsService, también tenés que registrar esas implementaciones en Program.cs.
    Creamos unos mocks de servicios de correo y SMS para simular el envío de notificaciones. En un proyecto real, estos servicios se conectarían a proveedores externos como SendGrid, Twilio, etc.

Paso 5: Una Vez testado con mocks, podés reemplazar las implementaciones de IEmailService e ISmsService por versiones reales que se integren con servicios de correo y SMS.

📧 Parte 1: Integración Real con Resend (Emails)
Resend es fantástico porque no necesitas instalar librerías pesadas; se maneja con una simple petición HTTP POST a su API enviando un JSON.

1. Obtener la API Key Gratuita
Entrá a resend.com y create una cuenta gratuita.

Ve a la sección API Keys y creá una nueva clave. Copiala.

Como estamos usando el plan gratuito sin un dominio propio, Resend te obligará a enviar los correos desde la dirección: onboarding@resend.dev. Y en tu base de datos, para las pruebas, el EmailDestino de tus eventos debe ser obligatoriamente tu propio correo real (el mismo con el que te registraste en Resend).


Parte 2: Integración Real con Twilio (SMS)
Para Twilio, en lugar de renegar con peticiones HTTP manuales, vamos a usar su paquete oficial para .NET, lo que hace que el código sea cortísimo.

1. Instalar el paquete de Twilio

Ejecutá el siguiente comando para instalar el paquete oficial de Twilio para .NET:

```dotnet add package Twilio
```

