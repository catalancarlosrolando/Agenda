import { Modal } from "./components/modal.js";

export class ModalEvento {
    constructor(service) {
        this.service = service;
    }

    async handleEvento(id = null) {
        this.limpiarBackdropsHuerfanos();

        const modalViejo = document.getElementById('exampleModal');
        if (modalViejo) {
            modalViejo.remove();
        }

        const modal = new Modal();
        let guardadoConExito = false;

        if (id) {
            const evento = await this.service.getEvento(id);
            const idEvento = evento.id;

            const html = modal.render(evento);
            document.body.insertAdjacentHTML('beforeend', html);
            const modalElement = document.getElementById('exampleModal');
            const bootstrapModal = new bootstrap.Modal(modalElement);

            this.configurarCierre(modalElement, () => guardadoConExito);

            bootstrapModal.show();

            guardadoConExito = await this.guardarEvento(bootstrapModal, idEvento);

        }
        else {
            const html = modal.render({ titulo: '', fechaHora: '', prioridad: 1, descripcion: '', telefonoDestino: '', emailDestino: '' });
            document.body.insertAdjacentHTML('beforeend', html);
            document.getElementById("exampleModalLabel").textContent = "Crear Nuevo Evento o no";
            document.getElementById("button-send").textContent = 'Enviar';
            const modalElement = document.getElementById('exampleModal');
            const bootstrapModal = new bootstrap.Modal(modalElement);

            this.configurarCierre(modalElement, () => guardadoConExito);

            bootstrapModal.show();
            guardadoConExito = await this.guardarEvento(bootstrapModal);

        }

    }

    async guardarEvento(bootstrapModal, idEvento = null) {
        const form = document.getElementById('form-modificar');
        return await new Promise((resolve) => {
            form.addEventListener('submit', async (e) => {
                e.preventDefault();

                try {
                    const formulario = e.target;
                    const formData = new FormData(formulario);
                    const datos = Object.fromEntries(formData.entries());
                    datos.prioridad = parseInt(datos.prioridad, 10);

                    if (idEvento) {
                        datos.id = idEvento;
                        await this.service.actualizarEvento(datos);
                    }
                    else {
                        const { titulo, fechaHora, prioridad, descripcion, telefonoDestino, emailDestino } = datos;
                        const eventoNuevo = { titulo, fechaHora, prioridad, descripcion, telefonoDestino, emailDestino };
                        await this.service.crearEvento(eventoNuevo);
                    }

                    document.activeElement?.blur();
                    bootstrapModal.hide();
                    resolve(true);
                } catch (error) {
                    console.error(error);
                    alert('No se pudo guardar el evento. Revisá los datos e intentá nuevamente.');
                    resolve(false);
                }
            }, { once: true });
        });
    }

    configurarCierre(modalElement, fueGuardado) {
        modalElement.addEventListener('hidden.bs.modal', () => {
            modalElement.remove();
            this.limpiarBackdropsHuerfanos();

            if (fueGuardado()) {
                window.app?.renderLista();
            }
        }, { once: true });
    }

    limpiarBackdropsHuerfanos() {
        document.querySelectorAll('.modal-backdrop').forEach((backdrop) => backdrop.remove());
        document.body.classList.remove('modal-open');
        document.body.style.removeProperty('padding-right');
    }
}