using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgendaApi.Data;
using AgendaApi.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

//Para validaciones
using AgendaApi.Features.Eventos;
using MediatR;

namespace AgendaApi.Controllers
{


    [ApiController]
    [Route("api/[controller]")]


    public class EventosController : ControllerBase
    {
        private readonly AgendaContext _context;
        private readonly IMediator _mediator;
        public EventosController(AgendaContext context)
        {
            _context = context;
        }

        public EventosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/eventos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evento>>> GetEventos()
        {
            return await _context.Eventos.OrderBy(e => e.FechaHora).ToListAsync();
        }

        // POST: api/eventos
        [HttpPost]
        public async Task<ActionResult<Evento>> PostEvento(Evento evento)
        {
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventos), new { id = evento.Id }, evento);
        }

        [HttpPost("manual")] // Ruta personalizada para este método
        public async Task<ActionResult<Evento>> PostEventoManual()
        {
            var form = await Request.ReadFormAsync();
            string titulo = form["titulo"];

            var messaje = string.Empty;

            if (titulo == null || titulo.Length < 5)
            {
                messaje = "El título es obligatorio y no puede exceder los 10 caracteres.";
            }

            else messaje = "Evento llego con éxito.";

            return Content(messaje); // Devolvemos el mensaje como texto plano  
        }

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

        [HttpGet("buscar/{titulo}")] // Agregamos un prefijo 'buscar' para no chocar con el ID
        public async Task<ActionResult<IEnumerable<Evento>>> GetEventoPorTitulo(string titulo)
        {
            // Usamos .Where para buscar coincidencias parciales (como un LIKE en SQL)
            var eventos = await _context.Eventos
                .Where(e => e.Titulo.ToLower().Contains(titulo.ToLower()))
                .ToListAsync();

            if (!eventos.Any())
            {
                return NotFound(new { mensaje = $"No se encontraron eventos con el título: {titulo}" });
            }

            return eventos;
        }

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
    }
}