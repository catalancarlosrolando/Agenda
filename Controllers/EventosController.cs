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

        private readonly IMediator _mediator;

        public EventosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //get api/eventos
        [HttpGet]
        public async Task<ActionResult> GetEventos()
        {
            // Traemos todos los eventos de la base de datos de forma asíncrona
            var eventos = await _mediator.Send(new ObtenerEventosQuery());
            return Ok(eventos);
        }

        //get api/eventos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetEvento(int id)
        {
            // Traemos el evento específico por ID de forma asíncrona
            var evento = await _mediator.Send(new ObtenerEventoPorIdQuery(id));

            if (evento == null)
                return NotFound(); // Si no existe, devolvemos un 404

            return Ok(evento); // Si existe, devolvemos el evento con un 200 OK
        }

        // POST: api/eventos
        [HttpPost]
        public async Task<ActionResult> PostEvento([FromBody] CrearEventoCommand comando)
        {
            // Enviamos el comando a MediatR. 
            // Aquí adentro se dispara AUTOMÁTICAMENTE el ValidationBehavior.
            // Si algo falla, el "tubo" frena la ejecución ANTES de llegar al Handler.
            var eventoId = await _mediator.Send(comando);

            // Retornamos un 201 Created indicando dónde consultar el recurso creado
            return CreatedAtAction("GetEvento", new { id = eventoId }, new { id = eventoId });

        }

        // PUT: api/eventos/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> PutEvento(int id, [FromBody] ActualizarEventoCommand comando)
        {
            if (id != comando.Id)
                return BadRequest("El ID del evento en la URL no coincide con el ID en el cuerpo de la solicitud.");

            var resultado = await _mediator.Send(comando);

            if (!resultado)
                return NotFound(); // Si no se encontró el evento para actualizar, devolvemos un 404

            return NoContent(); // Si se actualizó correctamente, devolvemos un 204 No Content
        }


        // DELETE: api/eventos/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvento(int id)
        {
            var eliminado = await _mediator.Send(new EliminarEventoCommand(id));
            if (!eliminado)
                return NotFound(); // Si no se encontró el evento para eliminar, devolvemos un 404

            return NoContent(); // Si se eliminó correctamente, devolvemos un 204 No Content
        }
    }

}