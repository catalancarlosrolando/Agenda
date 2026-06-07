1. Obtener un evento por ID (GET)
Para obtener una cita específica (por ejemplo, para ver los detalles de un examen), necesitamos pasar el Id en la URL: api/eventos/5.

Añade este método a tu EventosController:

C#
[HttpGet("{id}")] // El parámetro entre llaves indica que es variable
public async Task<ActionResult<Evento>> GetEvento(int id)
{
    // Buscamos en la base de datos
    var evento = await _context.Eventos.FindAsync(id);

    if (evento == null)
    {
        return NotFound(new { mensaje = $"No se encontró el evento con ID {id}" });
    }

    return evento;
}
¿Cómo funciona el mapeo?
El atributo [HttpGet("{id}")] le dice a .NET: "Busca un valor después de /api/eventos/".

.NET intenta convertir ese valor al tipo de dato que definiste en el parámetro del método (int id).

Seguridad automática: Si alguien intenta navegar a api/eventos/hola, el sistema devolverá un error 400 Bad Request automáticamente porque "hola" no es un número entero. No tienes que programar esa validación a mano.


2. Actualizar un evento (PUT)
Imagina que te cambiaron la fecha de un parcial. Para esto usamos el verbo PUT. Es una buena práctica validar que el ID de la URL coincida con el ID del objeto que envías en el cuerpo (Body).

C#
[HttpPut("{id}")]
public async Task<IActionResult> PutEvento(int id, Evento evento)
{
    if (id != evento.Id)
    {
        return BadRequest("El ID de la URL no coincide con el ID del cuerpo de la petición.");
    }

    _context.Entry(evento).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!_context.Eventos.Any(e => e.Id == id))
        {
            return NotFound();
        }
        throw;
    }

    return NoContent(); // 204: Se actualizó con éxito pero no devolvemos contenido
}



3. Eliminar un evento (DELETE)
Cuando un examen ya pasó o una cita se canceló, usamos DELETE.

C#
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteEvento(int id)
{
    var evento = await _context.Eventos.FindAsync(id);
    if (evento == null)
    {
        return NotFound();
    }

    _context.Eventos.Remove(evento);
    await _context.SaveChangesAsync();

    return NoContent();
}


Para avanzar con tu Agenda Online, el siguiente paso natural es poder consultar, actualizar o borrar un evento específico. Aquí es donde entra en juego el Ruteo con Parámetros.En ASP.NET Core, esto se logra extendiendo los atributos de los verbos HTTP.1. Obtener un evento por ID (GET)Para obtener una cita específica (por ejemplo, para ver los detalles de un examen), necesitamos pasar el Id en la URL: api/eventos/5.Añade este método a tu EventosController:C#[HttpGet("{id}")] // El parámetro entre llaves indica que es variable
public async Task<ActionResult<Evento>> GetEvento(int id)
{
    // Buscamos en la base de datos
    var evento = await _context.Eventos.FindAsync(id);

    if (evento == null)
    {
        return NotFound(new { mensaje = $"No se encontró el evento con ID {id}" });
    }

    return evento;
}
¿Cómo funciona el mapeo?El atributo [HttpGet("{id}")] le dice a .NET: "Busca un valor después de /api/eventos/"..NET intenta convertir ese valor al tipo de dato que definiste en el parámetro del método (int id).Seguridad automática: Si alguien intenta navegar a api/eventos/hola, el sistema devolverá un error 400 Bad Request automáticamente porque "hola" no es un número entero. No tienes que programar esa validación a mano.2. Actualizar un evento (PUT)Imagina que te cambiaron la fecha de un parcial. Para esto usamos el verbo PUT. Es una buena práctica validar que el ID de la URL coincida con el ID del objeto que envías en el cuerpo (Body).C#[HttpPut("{id}")]
public async Task<IActionResult> PutEvento(int id, Evento evento)
{
    if (id != evento.Id)
    {
        return BadRequest("El ID de la URL no coincide con el ID del cuerpo de la petición.");
    }

    _context.Entry(evento).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!_context.Eventos.Any(e => e.Id == id))
        {
            return NotFound();
        }
        throw;
    }

    return NoContent(); // 204: Se actualizó con éxito pero no devolvemos contenido
}
3. Eliminar un evento (DELETE)Cuando un examen ya pasó o una cita se canceló, usamos DELETE.C#[HttpDelete("{id}")]
public async Task<IActionResult> DeleteEvento(int id)
{
    var evento = await _context.Eventos.FindAsync(id);
    if (evento == null)
    {
        return NotFound();
    }

    _context.Eventos.Remove(evento);
    await _context.SaveChangesAsync();

    return NoContent();
}
4. Resumen de Rutas de tu AgendaCon estos cambios, tu API de Agenda ya tiene un CRUD completo. 

# Así se ven tus rutas ahora en RapidAPI o cualquier cliente:
Acción              Método          Ruta                Propósito           
Listar              GET             /api/eventos        Ver toda la agenda.
Ver uno             GET             /api/eventos/{id}   Ver detalle de una cita específica.
Crear               POST            /api/eventos        Crear un nuevo evento.
Editar              PUT             /api/eventos/{id}   Editar un evento existente.
Borrar              DELETE          /api/eventos/{id}   Eliminar un evento.
Con esta estructura, tu API es fácil de entender y usar. Cada ruta tiene un propósito claro y sigue las convenciones REST, lo que facilita su consumo desde cualquier cliente (web, móvil, etc.).

Como estás usando SQLite (o incluso en memoria), nota que estamos usando métodos Async. En .NET, esto es vital para que tu API no se "congele" mientras la base de datos responde, permitiendo que maneje múltiples peticiones al mismo tiempo de forma eficiente.


5. Podemos Utilizar los endpoints con react/vue etc. para mostrar la agenda en una interfaz gráfica, o incluso con una app móvil. La flexibilidad de tener una API RESTful te permite conectar tu backend con cualquier frontend que quieras desarrollar en el futuro.

o podemos integrar una interfaz para tu Agenda sin perder tus endpoints de API dentro del proyecto utilizando Razor pages o blazor.

El enfoque "Híbrido" (MVC + API)
Puedes transformar tu proyecto de Web API en uno que soporte Vistas Razor. En .NET, esto es muy común.

Paso A: Habilitar Vistas en Program.cs
Debes cambiar el servicio que registraste para que .NET busque tanto controladores de API como controladores con vistas.

C#
// Cambia builder.Services.AddControllers(); por:
builder.Services.AddControllersWithViews(); 
Paso B: La diferencia entre los Controladores
Ahora tendrás dos tipos de controladores en tu carpeta /Controllers:

EventosController (API): Hereda de ControllerBase. Devuelve JSON. Es el que ya tienes.

HomeController (Vistas): Hereda de Controller (a secas). Devuelve un View().

C#
public class HomeController : Controller
{
    // Esta ruta cargará la página principal en el navegador
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View(); // Esto busca un archivo llamado Index.cshtml
    }
}
2. ¿Cómo consume la Vista a la API?
Aquí es donde conectamos los cables. Tienes dos formas de hacerlo:

Opción A: Consumo desde el Cliente (JavaScript/AJAX)
Es la que más te servirá para tu Agenda porque permite actualizar la lista de citas sin recargar la página (lo que hablamos de AJAX al principio).

Creas una carpeta /Views/Home/ y dentro un archivo Index.cshtml.

En ese archivo escribes el HTML y un bloque de JavaScript que use fetch para llamar a tu propia API.

HTML
<h1>Mi Agenda Online</h1>
<div id="lista-eventos"></div>

<script>
    // Al cargar la página, llamamos a nuestro propio endpoint de API
    fetch('/api/eventos')
        .then(res => res.json())
        .then(data => {
            const div = document.getElementById('lista-eventos');
            data.forEach(evento => {
                div.innerHTML += `<p>${evento.titulo} - ${evento.fechaHora}</p>`;
            });
        });
</script>

Tienes toda la razón: tus controladores actuales heredan de ControllerBase, lo que significa que están diseñados puramente para datos (JSON). Para agregar "vistas" (interfaces gráficas) dentro del mismo proyecto de .NET, tienes dos caminos principales.

Aquí te explico cómo integrar una interfaz para tu Agenda sin perder tus endpoints de API.

1. El enfoque "Híbrido" (MVC + API)
Puedes transformar tu proyecto de Web API en uno que soporte Vistas Razor. En .NET, esto es muy común.

Paso A: Habilitar Vistas en Program.cs
Debes cambiar el servicio que registraste para que .NET busque tanto controladores de API como controladores con vistas.

C#
// Cambia builder.Services.AddControllers(); por:
builder.Services.AddControllersWithViews(); 
Paso B: La diferencia entre los Controladores
Ahora tendrás dos tipos de controladores en tu carpeta /Controllers:

EventosController (API): Hereda de ControllerBase. Devuelve JSON. Es el que ya tienes.

HomeController (Vistas): Hereda de Controller (a secas). Devuelve un View().

C#
public class HomeController : Controller
{
    // Esta ruta cargará la página principal en el navegador
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View(); // Esto busca un archivo llamado Index.cshtml
    }
}
2. ¿Cómo consume la Vista a la API?
Aquí es donde conectamos los cables. Tienes dos formas de hacerlo:

Opción A: Consumo desde el Cliente (JavaScript/AJAX)
Es la que más te servirá para tu Agenda porque permite actualizar la lista de citas sin recargar la página (lo que hablamos de AJAX al principio).

Creas una carpeta /Views/Home/ y dentro un archivo Index.cshtml.

En ese archivo escribes el HTML y un bloque de JavaScript que use fetch para llamar a tu propia API.

HTML
<h1>Mi Agenda Online</h1>
<div id="lista-eventos"></div>

<script>
    // Al cargar la página, llamamos a nuestro propio endpoint de API
    fetch('/api/eventos')
        .then(res => res.json())
        .then(data => {
            const div = document.getElementById('lista-eventos');
            data.forEach(evento => {
                div.innerHTML += `<p>${evento.titulo} - ${evento.fechaHora}</p>`;
            });
        });
</script>
3. Opción B: Consumo desde el Servidor (View Injection)
Si prefieres no usar tanto JavaScript y quieres que el servidor renderice todo (estilo Laravel con Blade), puedes inyectar el contexto de la base de datos directamente en el controlador de la vista.

C#
public class HomeController : Controller
{
    private readonly AgendaContext _context;

    public HomeController(AgendaContext context) { _context = context; }

    public async Task<IActionResult> Index()
    {
        // El servidor busca los datos y se los pasa a la vista antes de enviarla
        var eventos = await _context.Eventos.ToListAsync();
        return View(eventos); 
    }
}

# ¿Cuál es la Mejor Práctica?
Para una Agenda Moderna, lo ideal es el Enfoque A (API + JS). ¿Por qué?

Separación de responsabilidades: Tu API se encarga de los datos y tu Vista se encarga de cómo se ven.

Reutilización: Si mañana decides hacer la App móvil de tu agenda en Flutter, ya tienes los endpoints de la API listos y funcionando. No tienes que programar nada nuevo en el servidor.

Interactividad: Puedes agregar botones de "Borrar" o "Completar" que se ejecuten instantáneamente con AJAX, dando una sensación de aplicación profesional.

Resumen de la Estructura de Carpetas:

/Controllers

    EventosController.cs (API - JSON)
    HomeController.cs (Vista - HTML)

/Views
    /Home
        Index.cshtml (Tu diseño)
    /shared
        _Layout.cshtml (Plantilla común para todas las vistas)
    
    _ViewStart.cshtml (Configuración global de vistas)
    _ViewImports.cshtml (Importaciones comunes para vistas)

/wwwroot (Aquí guardarás tus archivos .css y .js externos)