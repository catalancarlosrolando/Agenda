import { Modal } from "../components/modal.js";

export class EventoServices {
    constructor(apiUrl) {
        this.API_URL = apiUrl;
    }

    async getEventos() {
        const res = await fetch(this.API_URL);
        const eventos = await res.json();
        return eventos;
    }

    async getEvento(id) {
        const res = await fetch(`${this.API_URL}/${id}`);
        const evento = await res.json();
        return evento;
    }


    async crearEvento(evento) {

        const res = await fetch(this.API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(evento)
        });
        if (res.ok) {
            return alert('Evento creado');
        }
        else {
            return alert('Error al crear evento');
        }
    }

    async eliminarEvento(id) {
        const res = await fetch(`${this.API_URL}/${id}`, { method: 'DELETE' });
        return res.ok;
    }

    async buscarEvento(titulo) {
        try {

            const res = await fetch(`${this.API_URL}/titulo/${titulo}`);
            if (res.ok) {
                return await res.json();
            }
            const errorData = await res.json().catch(() => null); // Intentamos parsear el error, pero si no es JSON, lo ignoramos
            if (errorData) {
                alert("Lo siento:\n" + errorData.mensaje);
                return
            }
        }
        catch (error) {
            console.error('Error al buscar el evento:', error);
            alert('Error al buscar el evento. Por favor, inténtalo de nuevo.');
        }
    }

    async actualizarEvento(datos) {
        try {
            // 5. Enviamos a la API
            const res = await fetch(`${this.API_URL}/${datos.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(datos)
            })

            if (res.ok) {
                return alert('Evento actualizado');
            }

            const errorData = await res.json().catch(() => null); // Intentamos parsear el error, pero si no es JSON, lo ignoramos
            if (errorData) {
                alert("Cuerpo completo del error:\n" + errorData.title);
            }
        }

        catch (error) {
            console.error('Error al actualizar el evento:', error);
            alert('Error al actualizar el evento. Por favor, inténtalo de nuevo.');
        };


    }

    async confirmarAlerta(id, estado) {
        try {
            const res = await fetch(`${this.API_URL}/${id}/confirmacion`, {
                method: 'PATCH',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(estado)
            });
            if (res.ok) {
                return true;
            }

            const errorData = await res.json().catch(() => null); // Intentamos parsear el error, pero si no es JSON, lo ignoramos
            if (errorData) {
                alert("Cuerpo completo del error:\n" + errorData.mensaje);
            }

        }
        catch (error) {
            alert('Error al actualizar el evento. Por favor, inténtalo de nuevo.');
        };



    }

}
