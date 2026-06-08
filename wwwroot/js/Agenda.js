import { EventoServices } from "./services/Services.js";
import { Home } from "./views/Home.js";
import { Modal } from "./components/modal.js";
import { router } from "./router/Router.js";
import { ModalEvento } from "./modalevento.js";

const API_URL = '/api/eventos';

export class AgendaApp {
    constructor() {
        this.eventoService = new EventoServices(API_URL);
        this.mainContainer = document.getElementById('main-content');
        this.homeEventos = new Home();
        this.router = new router(this);
        this.init();

    }

    init() {
        // 1. LEER LA RUTA AL CARGAR: 
        // Obtenemos lo que dice la barra de direcciones (ej: "/")
        const rutaInicial = window.location.pathname;

        // Ejecutamos la navegación inmediatamente
        // Agregamos un segundo parámetro 'false' para que no duplique la URL en el historial
        this.router.handleRouteChange(rutaInicial, false);

        // Configuramos los eventos del Navbar sin recargar la página
        document.querySelectorAll('.nav-link').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const ruta = e.currentTarget.getAttribute('href');
                this.router.handleRouteChange(ruta);
            });
        });
    }


    async renderLista() {
        const eventos = await this.eventoService.getEventos();

        this.mainContainer.innerHTML = this.homeEventos.render(eventos);
    }

    renderConfig() {
        this.mainContainer.innerHTML = `<h2>Contactos</h2><p>Ajustes de la agenda...</p>`;
    }

    // Eliminar evento
    async eliminarEvento(id) {
        if (confirm("¿Estás seguro de eliminar este evento?")) {
            const res = await this.eventoService.eliminarEvento(id);
            if (res) {
                alert('Evento eliminado');
            }
            this.renderLista();
        }
    }

    // Guardar nuevo evento
    async guardarEvento() {
        const modalEvento = new ModalEvento(this.eventoService);
        await modalEvento.handleEvento();
        //await this.eventoService.crearEvento();
        this.renderLista();
    }

    // Modificar evento
    async actualizarEvento(id) {
        const modalEvento = new ModalEvento(this.eventoService);
        await modalEvento.handleEvento(id);
    }

    async buscarEvento() {
        const query = document.getElementById('titulo').value;
        const eventos = await this.eventoService.buscarEvento(query);
        this.mainContainer.innerHTML = this.homeEventos.render(eventos);
    }

    async confirmarAlerta(id, estado, boton) {
        const eventos = await this.eventoService.confirmarAlerta(id, estado);
        if (eventos) {

            boton.className = "btn btn-sm btn-outline-success border-2 rounded-circle p-2 d-inline-flex align-items-center justify-content-center";
            boton.style.backgroundColor = "rgba(25, 135, 84, 0.1)";
            boton.setAttribute('data-estado', 'true');
            boton.setAttribute('title', 'Alerta Confirmada');

        }

    }
}

// Inicializamos la app
const app = new AgendaApp();
window.app = app; // Hacemos global para poder llamar desde los botones de los cards