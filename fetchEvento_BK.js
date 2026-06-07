const API_URL = '/api/eventos';

const API_URL_BUSCAR = '/api/eventos/buscar';

let modalEdicion;

function obtenerInstanciaModal() {
    const el = document.getElementById('exampleModal');
    // Si ya existe la instancia, la devolvemos. Si no, la creamos.
    if (!modalEdicion) {
        modalEdicion = new bootstrap.Modal(el);
    }
    return modalEdicion;
}

// 1. Cargar eventos al iniciar
async function cargarEventos() {
    const res = await fetch(API_URL);
    const eventos = await res.json();
    const contenedor = document.getElementById('contenedor-eventos');
    contenedor.innerHTML = '';
    CrearEventos(eventos, contenedor);
}

function CrearEventos(eventos, contenedor) {
    eventos.forEach(ev => {
        const fechaFormat = new Date(ev.fechaHora).toLocaleString();
        const prioridadClass = ev.prioridad === 3 ? 'danger' : (ev.prioridad === 2 ? 'warning' : 'info');

        contenedor.innerHTML += `
                                <div class="col-md-4 mb-3">
                                    <div class="card border-${prioridadClass} shadow-sm">
                                        <div class="card-body">
                                            <h5 class="card-title">${ev.titulo}</h5>
                                            <p class="card-text text-muted">${fechaFormat}</p>
                                            <span class="badge bg-${prioridadClass}">${ev.tipo || 'Evento'}</span>
                                            <button onclick="eliminarEvento(${ev.id})" class="btn btn-sm btn-outline-danger float-end ms-1">Borrar</button>
                                            <button onclick="actualizarEvento(${ev.id})" class="btn btn-sm btn-outline-danger float-end">Modificar</button>
                                        </div>
                                    </div>
                                </div>
                            `;
    });
}

// 2. Guardar nuevo evento
async function guardarEvento() {
    const nuevo = {
        titulo: document.getElementById('titulo').value,
        fechaHora: document.getElementById('fecha').value,
        prioridad: parseInt(document.getElementById('prioridad').value)
    };

    const res = await fetch(API_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(nuevo)
    });

    if (res.ok) {
        document.getElementById('titulo').value = '';
        cargarEventos();
    }
}

// 3. Eliminar evento
async function eliminarEvento(id) {
    if (confirm('¿Seguro que quieres borrar este evento?')) {
        const res = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
        if (res.ok) {
            alert('Evento eliminado');
        }
        cargarEventos();
    }
}

// 4. Buscar evento
async function buscarEvento() {
    const query = document.getElementById('titulo').value;
    const res = await fetch(`${API_URL_BUSCAR}/${query}`);
    const eventos = await res.json();
    console.log("Eventos por buscar", eventos);
    const contenedor = document.getElementById('contenedor-eventos');
    contenedor.innerHTML = '';
    CrearEventos(eventos, contenedor);
    document.getElementById('titulo').value = '';
}

// 5. Modificar evento
async function actualizarEvento(id) {
    //mostrar modal con datos del evento
    const res = await fetch(`${API_URL}/${id}`);
    const evento = await res.json();
    const idEvento = evento.id;
    document.getElementById('modificar-titulo').value = evento.titulo;
    document.getElementById('modificar-fecha').value = new Date(evento.fechaHora).toISOString().slice(0, 16);
    document.getElementById('modificar-prioridad').value = evento.pPrioridad;
    obtenerInstanciaModal().show();

    document.getElementById('form-modificar').addEventListener('submit', async function (e) {
        e.preventDefault(); // Evitamos que la página se recargue

        // 1. Capturamos el formulario
        const formulario = e.target;
        console.log(formulario);

        // 2. Creamos el objeto FormData
        const formData = new FormData(formulario);

        // 3. Convertimos FormData a un Objeto JSON plano para .NET
        const datos = Object.fromEntries(formData.entries());

        // 4. Ajustes de tipos (FormData siempre devuelve strings)
        datos.id = idEvento;
        datos.prioridad = parseInt(datos.prioridad);

        // 5. Enviamos a la API
        const res = await fetch(`${API_URL}/${datos.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(datos)
        });

        if (res.ok) {
            // Cerramos modal y refrescamos
            document.activeElement.blur()
            obtenerInstanciaModal().hide();
            cargarEventos();
        }
    });
};



cargarEventos();

