import { Modal } from "./components/modal.js";

export class ModalEvento {
    constructor(service) {
        this.modalContainer = document.getElementById('modal-container');
        this.service = service;
    }

    async handleEvento(id = null) {
        const modalViejo = document.getElementById('exampleModal');
        if (modalViejo) modalViejo.remove();
        // 3. Creamos el modal
        const modal = new Modal();
        if (id) {
            const evento = await this.service.getEvento(id);
            const idEvento = evento.id;
            console.log(evento);

            // 2. Eliminamos cualquier modal previo para no duplicar IDs en el DOM

            const html = modal.render(evento);
            app.mainContainer.insertAdjacentHTML('beforeend', html);
            const modalElement = document.getElementById('exampleModal');
            const bootstrapModal = new bootstrap.Modal(modalElement);

            bootstrapModal.show();

            // 4. Escuchamos el submit del formulario dentro del modal y usamos arrow function para mantener el contexto de "this"

            await this.guardarEvento(bootstrapModal, idEvento);

        }
        else {
            //cargar Evento vacío para crear nuevo
            const html = modal.render({ titulo: '', fechaHora: '', prioridad: 1, descripcion: '', telefonoDestino: '', emailDestino: '' });
            app.mainContainer.insertAdjacentHTML('beforeend', html);
            document.getElementById("exampleModalLabel").textContent = "Crear Nuevo Evento o no";
            document.getElementById("button-send").textContent = 'Enviar';
            const modalElement = document.getElementById('exampleModal');
            const bootstrapModal = new bootstrap.Modal(modalElement);
            bootstrapModal.show();
            await this.guardarEvento(bootstrapModal);

        }

    }

    async guardarEvento(bootstrapModal, idEvento = null) {
        const form = document.getElementById('form-modificar');
        await form.addEventListener('submit', async (e) => {
            e.preventDefault(); // Evitamos que la página se recargue

            // 1. Capturamos el formulario
            const formulario = e.target;

            // 2. Creamos el objeto FormData
            const formData = new FormData(formulario);

            // 3. Convertimos FormData a un Objeto JSON plano para .NET
            const datos = Object.fromEntries(formData.entries());

            // 4. Ajustes de tipos (FormData siempre devuelve strings)
            datos.prioridad = parseInt(datos.prioridad);

            if (idEvento) {
                // Si hay ID, es una actualización
                datos.id = idEvento;
                const res = await this.service.actualizarEvento(datos);

            }
            else {
                // Si no hay ID, es una creación
                const { titulo, fechaHora, prioridad, descripcion, telefonoDestino, emailDestino } = datos;
                const eventoNuevo = { titulo, fechaHora, prioridad, descripcion, telefonoDestino, emailDestino };
                await this.service.crearEvento(eventoNuevo);
            }
            // Cerramos modal y refrescamos la lista
            document.activeElement.blur();
            bootstrapModal.hide();
            app.renderLista();
        });
    }
}