# Documentación Completa del Proyecto Agenda

## 1. Creación del Proyecto

### Framework y Entorno
- **Tipo de Proyecto**: ASP.NET Core MVC Web Application
- **Framework**: .NET 10.0 (net10.0)
- **Patrón Arquitectónico**: MVC + CQRS (Command Query Responsibility Segregation)
- **Archivo de Configuración**: `Agenda.csproj`

### Características del Proyecto
- **Null-safety**: Habilitado (`<Nullable>enable</Nullable>`)
- **Using Globales**: Habilitados (`<ImplicitUsings>enable</ImplicitUsings>`)

---

## 2. Dependencias Tecnológicas

### Paquetes NuGet Instalados

1. **FluentValidation.DependencyInjectionExtensions** (v12.1.1)
   - Validación de modelos con reglas de negocio en español
   - Integración con inyección de dependencias

2. **MediatR** (v14.1.0)
   - Implementación del patrón CQRS
   - Manejo de Comandos (Commands) y Consultas (Queries)
   - Pipeline de comportamientos (Behaviors)

3. **Microsoft.EntityFrameworkCore.Sqlite** (v10.0.6)
   - ORM para acceso a base de datos
   - Soporte para SQLite

4. **Microsoft.EntityFrameworkCore.Design** (v10.0.6)
   - Herramientas de diseño para migraciones
   - Scaffolding y reverse engineering

5. **Microsoft.AspNetCore.OpenApi** (v10.0.6)
   - Soporte para OpenAPI / Swagger

6. **NSwag.AspNetCore** (v14.7.1)
   - Generación automática de documentación API
   - Interfaz Swagger mejorada

7. **Scalar.AspNetCore** (v2.14.14)
   - Documentación interactiva de API (alternativa a Swagger UI)

8. **Twilio** (v7.14.9)
   - Servicio de envío de SMS para notificaciones críticas
   - Autenticación y manejo de credenciales

---

## 3. Base de Datos

### Tipo de Base de Datos
- **Motor**: SQLite
- **Archivo de BD**: `agenda.db` (persistente en el sistema de archivos)
- **Ubicación**: Raíz del proyecto

### Contexto de Base de Datos
- **Clase**: `AgendaContext`
- **Ubicación**: `Data/AgendaContext.cs`
- **DbSet**: 
  - `DbSet<Evento> Eventos` - Tabla principal de eventos

### Migraciones
- **Migración 1**: `20260419230140_MigracionInicial`
  - Crea la estructura inicial de la tabla Eventos
  
- **Migración 2**: `20260607005355_EvolucionModeloEventosParaNotificaciones`
  - Agrega campos para notificaciones:
    - EmailDestino
    - TelefonoDestino
    - MailEnviado
    - SmsEnviado
    - AlertaConfirmada

---

## 4. Modelos

### Evento (`Models/Evento.cs`)
Entidad principal que representa un evento en la agenda.

**Propiedades**:
- `int Id` - Identificador único
- `string Titulo` - Título del evento (obligatorio)
- `string? Descripcion` - Descripción del evento (opcional)
- `DateTime FechaHora` - Fecha y hora del evento
- `string? Tipo` - Tipo/Categoría del evento (opcional)
- `int Prioridad` - Nivel de prioridad (1=Baja, 2=Media, 3=Alta)
- `string EmailDestino` - Email para envío de notificación (obligatorio)
- `string TelefonoDestino` - Teléfono para envío de SMS (obligatorio)
- `bool MailEnviado` - Flag indicador de email enviado
- `bool SmsEnviado` - Flag indicador de SMS enviado
- `bool AlertaConfirmada` - Flag de confirmación de alerta por usuario

---

## 5. Controladores

### EventosController (`Controllers/EventosController.cs`)
Controlador API principal para gestión de eventos.

**Ruta Base**: `api/eventos`

**Endpoints Disponibles**:
1. **GET /api/eventos**
   - Query: `ObtenerEventosQuery()`
   - Retorna lista completa de eventos

2. **GET /api/eventos/{id}**
   - Query: `ObtenerEventoPorIdQuery(id)`
   - Retorna un evento específico por ID

3. **POST /api/eventos**
   - Command: `CrearEventoCommand(...)`
   - Crea un nuevo evento en la base de datos

4. **PUT /api/eventos/{id}**
   - Command: `ActualizarEventoCommand(...)`
   - Actualiza un evento existente

5. **DELETE /api/eventos/{id}**
   - Command: `EliminarEventoCommand(id)`
   - Elimina un evento

6. **POST /api/eventos/{id}/confirmar-alerta**
   - Command: `ConfirmarAlertaCommand(id)`
   - Marca una alerta como confirmada por el usuario (detiene escalamiento a SMS)

**Inyección de Dependencias**:
- `IMediator` - Para envío de commands y queries
- `AgendaContext` - Acceso directo a BD (como respaldo)

### HomeController (`Controllers/HomeController.cs`)
Controlador para servir la interfaz de usuario.

**Rutas**:
- **GET /** - Retorna la vista Index.cshtml (SPA)

---

## 6. Features - Patrón CQRS

### Queries (Consultas)

#### ObtenerEventosQuery
- **Ubicación**: `Features/ObtenerEventosQuery.cs`
- **Handler**: `ObtenerEventosHandler.cs`
- **Retorno**: `List<Evento>`
- **Descripción**: Obtiene todos los eventos de la BD

#### ObtenerEventoPorIdQuery
- **Ubicación**: `Features/ObtenerEventoPorIdQuery.cs`
- **Handler**: `ObtenerEventosHandler.cs`
- **Parámetro**: `int Id`
- **Retorno**: `Evento?` (puede ser null)
- **Descripción**: Obtiene un evento específico por su ID

### Comandos (Creación y Modificación)

#### CrearEventoCommand
- **Ubicación**: `Features/CrearEventoCommand.cs`
- **Handler**: `CrearEventoHandler.cs`
- **Validador**: `CrearEventoValidator.cs`
- **Parámetros**:
  - `string Titulo`
  - `string? Descripcion`
  - `DateTime FechaHora`
  - `string? Tipo`
  - `int Prioridad`
  - `string EmailDestino`
  - `string TelefonoDestino`
- **Retorno**: `int` (ID del evento creado)
- **Descripción**: Crea un nuevo evento y lo encola para notificaciones

#### ActualizarEventoCommand
- **Ubicación**: `Features/ActualizarEventoCommand.cs`
- **Handler**: `AcualizarEventoHandler.cs`
- **Validador**: `ActualizarEventoValidator.cs`
- **Parámetros**: 
  - `int Id`
  - `string Titulo`
  - `string? Descripcion`
  - `DateTime FechaHora`
  - `string? Tipo`
  - `int Prioridad`
  - `string EmailDestino`
  - `string TelefonoDestino`
  - `bool AlertaConfirmada`
- **Retorno**: `bool` (true si se actualizó, false si no existe)
- **Descripción**: Actualiza un evento existente

#### EliminarEventoCommand
- **Ubicación**: `Features/EliminarEventoCommand.cs`
- **Handler**: `EliminarEventoHandler.cs`
- **Parámetro**: `int Id`
- **Retorno**: `bool` (true si se eliminó, false si no existe)
- **Descripción**: Elimina un evento de la BD

#### ConfirmarAlertaCommand
- **Ubicación**: `Features/ConfirmarAlertaComand.cs`
- **Handler**: `ConfirmarAlertaHandler.cs`
- **Parámetro**: `int EventoId`
- **Retorno**: `bool`
- **Descripción**: Marca una alerta como confirmada, deteniendo el escalamiento a SMS

### Validadores

#### CrearEventoValidator
- Validación de campos requeridos
- Validación de formato de email
- Validación de formato de teléfono

#### ActualizarEventoValidator
- Idem CrearEventoValidator con validaciones adicionales

---

## 7. Servicios

### Interfaces (`Services/Interfaces.cs`)

```csharp
public interface IEmailService
{
    Task EnviarAlertaAsync(string destino, string tituloEvento, DateTime fechaHora);
}

public interface ISmsService
{
    Task EnviarAlertaCriticaAsync(string destino, string tituloEvento, DateTime fechaHora);
}
```

### ResendEmailService (`Services/ResendEmailService.cs`)
- **Implementa**: `IEmailService`
- **Responsabilidad**: Envío de notificaciones por email
- **Proveedor**: Resend API
- **Configuración**: HttpClient registrado con inyección de dependencias
- **Método**: `EnviarAlertaAsync(destino, tituloEvento, fechaHora)`

### TwilioSmsService (`Services/TwilioSmsService.cs`)
- **Implementa**: `ISmsService`
- **Responsabilidad**: Envío de notificaciones por SMS
- **Proveedor**: Twilio
- **Configuración**: Scoped (una instancia por request)
- **Método**: `EnviarAlertaCriticaAsync(destino, tituloEvento, fechaHora)`

### NotificationQueue (`Services/NotificationQueue.cs`)
- **Tipo**: Cola de canales (Channel<T>)
- **Responsabilidad**: Almacenamiento en memoria de eventos pendientes de notificar
- **Patrón**: Productor-Consumidor
- **Interfaz**: `INotificationQueue`
- **Métodos**:
  - `EnqueueAsync(evento)` - Agrega evento a la cola
  - `DequeueAsync(token)` - Obtiene evento de la cola (bloqueante)

### NotificationWorker (`Services/NotificationWorker.cs`)
- **Hereda de**: `BackgroundService`
- **Responsabilidad**: Procesamiento de notificaciones en segundo plano
- **Ejecución**: Automática al iniciar la aplicación
- **Inyecciones**: `INotificationQueue`, `IServiceScopeFactory`, `ILogger`

#### Estructura de Ejecución

**ExecuteAsync()** - Punto de entrada del worker
- Inicia logging: "Motor de Notificaciones Inteligentes iniciado..."
- Lanza dos tareas paralelas simultáneamente:
  1. `EscucharColaEventosAsync(stoppingToken)`
  2. `ServidorRelojEscalamientoAsync(stoppingToken)`
- Espera a que ambas tareas completen con `Task.WhenAll()`

#### TAREA 1: EscucharColaEventosAsync()

**Responsabilidad**: Reacciona inmediatamente cuando entra un evento a la cola

**Funcionamiento**:
- Bucle infinito mientras `!stoppingToken.IsCancellationRequested`
- Espera de forma **asíncrona sin consumir CPU** a que caiga un ID en la cola
- Llama a `_queue.LeerEventoAsync(stoppingToken)` - Bloqueante
- Log: "🔥 Evento detectado en la cola (ID: {Id}). Planificando alertas..."
- Manejo de excepciones:
  - `OperationCanceledException` → Rompe el bucle
  - Excepciones genéricas → Registra error y continúa

**Notas**:
- Actualmente solo detecta y registra el evento
- Punto de extensión para lógica adicional futura
- No consume recursos mientras espera (await en Channel)

#### TAREA 2: ServidorRelojEscalamientoAsync()

**Responsabilidad**: Revisa la base de datos cada 60 segundos para ejecutar el motor de escalamiento

**Ciclo de Ejecución**:
1. Log: "⏰ Reloj de Escalamiento: Revisando eventos próximos en la base de datos..."
2. Crea un `Scope` temporal para acceder a `DbContext` de forma segura
3. Obtiene servicios del scope:
   - `AgendaContext` - Acceso a BD
   - `IEmailService` - Envío de emails
   - `ISmsService` - Envío de SMS

4. **Consulta de Eventos**:
   ```csharp
   var eventosProximos = await context.Eventos
       .Where(e => e.FechaHora > ahora && !e.AlertaConfirmada)
       .ToListAsync(stoppingToken);
   ```
   - Filtra eventos con fecha futura
   - Excluye eventos con alerta confirmada

5. Para cada evento, calcula: `tiempoRestante = FechaHora - Ahora`

#### Motor de Escalamiento Inteligente

**Escalón 1: Alerta por Email (Menos de 24 horas)**
```
Condiciones:
- tiempoRestante.TotalHours <= 24
- MailEnviado == false
- AlertaConfirmada == false

Acción:
1. Log: "📧 Disparando Escalón 1 (Mail) para el evento: {Titulo}"
2. Ejecuta: await emailService.EnviarAlertaAsync(ev.EmailDestino, ev.Titulo, ev.FechaHora)
3. Establece: ev.MailEnviado = true
4. Marca entrada como modificada: context.Entry(ev).Property(x => x.MailEnviado).IsModified = true

Resultado:
- Usuario recibe email de advertencia
- Alerta temprana (24 horas antes)
```

**Escalón 2: Alerta Crítica por SMS (Menos de 2 horas)**
```
Condiciones:
- tiempoRestante.TotalHours <= 2
- SmsEnviado == false
- AlertaConfirmada == false (si es true, se detiene aquí)

Acción:
1. Log: "🚨 ESCALAMIENTO DISPARADO (SMS de emergencia) para el evento: {Titulo}"
2. Ejecuta: await smsService.EnviarAlertaCriticaAsync(ev.TelefonoDestino, ev.Titulo, ev.FechaHora)
3. Establece: ev.SmsEnviado = true
4. Marca entrada como modificada: context.Entry(ev).Property(x => x.SmsEnviado).IsModified = true

Resultado:
- Usuario recibe SMS de emergencia (solo si no confirmó email)
- Alerta final crítica (2 horas antes del evento)
```

**Persistencia de Cambios**:
- Después de procesar todos los eventos: `await context.SaveChangesAsync(stoppingToken)`
- Actualiza flags `MailEnviado` y `SmsEnviado` en la BD
- Evita duplicación de notificaciones

**Ciclo de Espera**:
- `await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken)`
- Espera exactamente 1 minuto (asíncrona, sin bloqueos)
- Vuelve al paso 1

#### Ejemplo de Flujo Completo

```
Evento creado: "Reunión importante"
FechaHora: 2026-06-10 14:00:00
Ahora: 2026-06-09 14:00:00 (24 horas antes)

Ciclo 1 (Minuto 1):
  ✓ tiempoRestante = 24 horas
  ✓ MailEnviado = false
  → DISPARA EMAIL
  → MailEnviado = true (guardado en BD)
  → Espera 1 minuto

Ciclo 2-59 (Minutos 2-59):
  ✓ tiempoRestante = 23:59 a 23:01 horas
  ✗ MailEnviado = true (ya no cumple condición)
  → No hace nada
  → Espera 1 minuto (continúa revisando)

Ciclo 60 (Minuto 60, faltan 23 horas):
  ✗ tiempoRestante > 2 horas
  ✗ SmsEnviado condition no aplica
  → No hace nada
  → Espera 1 minuto

... más ciclos hasta que tiempoRestante <= 2 horas ...

Ciclo N (Faltan 1:59 horas):
  ✓ tiempoRestante = 1:59 horas
  ✓ SmsEnviado = false
  ✓ AlertaConfirmada = false
  → DISPARA SMS
  → SmsEnviado = true (guardado en BD)
  → Espera 1 minuto

Si usuario confirma alerta antes (AlertaConfirmada = true):
  Ciclo siguiente:
  ✓ AlertaConfirmada = true
  → Se salta SMS
  → Motor de escalamiento finaliza
```

#### Casos de Detención del Escalamiento

1. **Usuario confirma alerta**:
   - Endpoint `POST /api/eventos/{id}/confirmar-alerta` establece `AlertaConfirmada = true`
   - El reloj ve esta bandera y detiene el SMS

2. **Evento ya pasó**:
   - Reloj filtra con `e.FechaHora > ahora`
   - Eventos pasados no se procesan

3. **Aplicación se detiene**:
   - `stoppingToken.IsCancellationRequested` rompe los bucles
   - Las tareas terminan gracefully

### MockServices (`Services/MockServices.cs`)
- Servicios de prueba para desarrollo sin credenciales reales
- Simula comportamientos de Email y SMS

---

## 8. Comportamientos (Pipeline CQRS)

### ValidationBehavior (`Behaviors/ValidationBehavior.cs`)
- **Tipo**: `IPipelineBehavior<TRequest, TResponse>`
- **Responsabilidad**: Validación automática antes de ejecutar handlers
- **Integración**: FluentValidation
- **Lanzamiento**: `ValidationException` si hay errores
- **Registración**: En `Program.cs` como behavior abierto del pipeline CQRS

---

## 9. Manejo de Excepciones

### GlobalExceptionHandler (`Exceptions/GlobalExceptionHandler.cs`)
- **Implementa**: `IExceptionHandler`
- **Responsabilidad**: Manejo centralizado de excepciones no controladas
- **Excepciones Manejadas**:
  - `ValidationException` - Errores de validación con formato estándar
  - Excepciones genéricas - Logging y respuesta estandarizada

**Respuesta de Errores de Validación**:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Errores de validación de negocio.",
  "status": 400,
  "detail": "Un o más campos no cumplen con las reglas del sistema.",
  "instance": "/api/eventos",
  "errors": {
    "Titulo": ["El título es requerido"],
    "EmailDestino": ["Email inválido"]
  }
}
```

---

## 10. Configuración del Proyecto

### Program.cs - Inicialización de Servicios

1. **Excepciones Globales**
   ```csharp
   builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
   builder.Services.AddProblemDetails();
   ```

2. **Base de Datos**
   ```csharp
   builder.Services.AddDbContext<AgendaContext>(options =>
       options.UseSqlite("Data Source=agenda.db"));
   ```

3. **Validación**
   ```csharp
   builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
   ```

4. **MediatR y CQRS**
   ```csharp
   builder.Services.AddMediatR(cfg =>
   {
       cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
       cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
   });
   ```

5. **Cola de Notificaciones**
   ```csharp
   builder.Services.AddSingleton<INotificationQueue, NotificationQueue>();
   ```

6. **Worker en Segundo Plano**
   ```csharp
   builder.Services.AddHostedService<NotificationWorker>();
   ```

7. **Servicios de Notificación**
   ```csharp
   builder.Services.AddHttpClient<IEmailService, ResendEmailService>();
   builder.Services.AddScoped<ISmsService, TwilioSmsService>();
   ```

8. **Configuración de Middleware**
   - Manejo de excepciones
   - Servicio de archivos estáticos (wwwroot)
   - Rutas controladores
   - Rutas vistas MVC
   - Swagger/OpenAPI en desarrollo
   - HTTPS redirection
   - Autorización
   - Fallback para SPA (Index.html)

---

## 11. Frontend

### Estructura de Archivos Estáticos

#### HTML Principal
- **Ubicación**: `wwwroot/Index.html`
- **Tipo**: SPA (Single Page Application)
- **Router**: Implementado en JavaScript

#### Estilos CSS
- **Ubicación**: `wwwroot/css/`
- **Archivo Principal**: `agenda.css`
- **Descripción**: Estilos de la aplicación

#### Scripts JavaScript
- **Ubicación**: `wwwroot/js/`

**Archivos Principales**:
1. `Agenda.js` - Archivo principal de la aplicación
2. `modalevento.js` - Gestión de modales para eventos

**Carpeta `components/`**:
- `EventoCard.js` - Componente para mostrar tarjetas de eventos
- `Busqueda.js` - Componente de búsqueda/filtrado

**Carpeta `router/`**:
- Implementación del enrutamiento SPA
- Manejo de navegación entre vistas

**Carpeta `services/`**:
- Servicios de comunicación con API
- Llamadas HTTP a endpoints

**Carpeta `views/`**:
- Componentes de vista reutilizables

#### Librerías
- **Ubicación**: `wwwroot/lib/`
- Librerías de terceros (jQuery, Bootstrap, etc.)

---

## 12. Vistas MVC (Razor)

### _ViewStart.cshtml
- Establece layout predeterminado
- Se ejecuta antes de cada vista

### _ViewImports.cshtml
- Importaciones globales
- Directivas comunes para todas las vistas

### _Layout.cshtml (`Views/Shared/`)
- Layout maestro de la aplicación
- Define estructura HTML principal
- Incluye referencias a CSS y JS

### Index.cshtml (`Views/Home/`)
- Página principal
- Carga el SPA (Index.html)

---

## 13. Configuración de Desarrollo

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json
- Configuraciones específicas para desarrollo
- Variables de entorno sensibles

### Properties/launchSettings.json
- Configuración de perfiles de ejecución
- Variables de entorno
- Puerto HTTPS

---

## 14. Funcionalidades del Sistema

### 1. Gestión de Eventos
- Crear nuevos eventos
- Listar todos los eventos
- Obtener detalles de evento específico
- Actualizar eventos
- Eliminar eventos

### 2. Sistema de Notificaciones Inteligentes
- **Motor de Escalamiento**:
  - En la fecha programada → Envía email
  - Pasado tiempo X sin confirmación → Envía SMS
  - Usuario confirma alerta → Detiene escalamiento

- **Canales de Notificación**:
  - Email via Resend API
  - SMS via Twilio

### 3. Validación de Negocio
- Validación de campos requeridos
- Validación de formato de email
- Validación de formato de teléfono
- Validación de prioridades
- Mensajes de error en español

### 4. API REST Documentada
- OpenAPI/Swagger disponible
- Documentación interactiva con Scalar
- CORS configurado
- Manejo de errores estandarizado

### 5. Interfaz de Usuario
- SPA con navegación sin recarga
- Componentes dinámicos de eventos
- Modales para crear/editar eventos
- Búsqueda y filtrado de eventos
- Diseño responsivo

---

## 15. Flujo de Creación de un Evento

```
1. Usuario rellena formulario en frontend
       ↓
2. Validación en cliente (JavaScript)
       ↓
3. POST /api/eventos con CrearEventoCommand
       ↓
4. ValidationBehavior valida reglas de negocio
       ↓
5. CrearEventoHandler persiste en BD
       ↓
6. Evento se encola en NotificationQueue
       ↓
7. NotificationWorker (background) procesa:
   a. Espera hasta FechaHora
   b. Envía email a EmailDestino
   c. Guarda MailEnviado=true
   d. Espera confirmación por tiempo X
   e. Si no confirma → Envía SMS a TelefonoDestino
   f. Si confirma (AlertaConfirmada=true) → Detiene
       ↓
8. Respuesta al cliente con ID del evento
```

---

## 16. Stack Tecnológico Resumido

| Capa | Tecnología |
|------|-----------|
| **Backend Framework** | ASP.NET Core 10.0 (MVC) |
| **Patrón Arquitectónico** | CQRS + MediatR |
| **Base de Datos** | SQLite 10.0.6 |
| **ORM** | Entity Framework Core 10.0.6 |
| **Validación** | FluentValidation 12.1.1 |
| **Email** | Resend API |
| **SMS** | Twilio 7.14.9 |
| **API Documentation** | Swagger + Scalar 2.14.14 |
| **Frontend** | HTML5, CSS3, JavaScript (Vanilla SPA) |
| **HTTP Client** | Fetch API (navegador) / HttpClient (.NET) |

---

## 17. Estructura de Directorios Final

```
Agenda/
├── Behaviors/              # Pipeline CQRS
│   └── ValidationBehavior.cs
├── Controllers/            # Controladores MVC/API
│   ├── EventosController.cs
│   └── HomeController.cs
├── Data/                   # Contexto de BD
│   └── AgendaContext.cs
├── Exceptions/             # Manejo centralizado
│   └── GlobalExceptionHandler.cs
├── Features/               # CQRS (Commands, Queries, Handlers, Validators)
│   ├── CrearEventoCommand.cs
│   ├── CrearEventoHandler.cs
│   ├── CrearEventoValidator.cs
│   ├── ActualizarEventoCommand.cs
│   ├── AcualizarEventoHandler.cs
│   ├── ActualizarEventoValidator.cs
│   ├── EliminarEventoCommand.cs
│   ├── EliminarEventoHandler.cs
│   ├── ConfirmarAlertaComand.cs
│   ├── ConfirmarAlertaHandler.cs
│   ├── ObtenerEventosQuery.cs
│   ├── ObtenerEventoPorIdQuery.cs
│   └── ObtenerEventosHandler.cs
├── Migrations/             # Migraciones de BD
├── Models/                 # Entidades
│   └── Evento.cs
├── Properties/             # Configuración
│   └── launchSettings.json
├── Services/               # Servicios de negocio
│   ├── Interfaces.cs
│   ├── NotificationQueue.cs
│   ├── NotificationWorker.cs
│   ├── ResendEmailService.cs
│   ├── TwilioSmsService.cs
│   └── MockServices.cs
├── Views/                  # Vistas Razor
│   ├── Home/
│   │   └── Index.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _ViewStart.cshtml
│       └── _ViewImports.cshtml
├── wwwroot/                # Archivos estáticos (Frontend SPA)
│   ├── Index.html
│   ├── css/
│   │   └── agenda.css
│   ├── js/
│   │   ├── Agenda.js
│   │   ├── modalevento.js
│   │   ├── components/
│   │   ├── router/
│   │   ├── services/
│   │   └── views/
│   ├── assets/
│   └── lib/
├── Agenda.csproj           # Definición del proyecto
├── Agenda.sln              # Solución
├── Program.cs              # Punto de entrada
├── appsettings.json        # Configuración
├── appsettings.Development.json
└── agenda.db               # Base de datos SQLite
```

---

## Conclusión

El proyecto **Agenda** es una aplicación moderna construida sobre ASP.NET Core que implementa el patrón CQRS para una separación clara de responsabilidades. Combina un backend robusto con servicios de notificación inteligentes (email y SMS) y un frontend SPA interactivo, proporcionando una solución completa para la gestión de eventos con alertas escalonadas.
