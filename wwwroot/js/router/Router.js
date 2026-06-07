export class router {
    constructor(app) {
        this.app = app;
        window.onpopstate = () => this.handleRouteChange(window.location.pathname);
    }

    async handleRouteChange(ruta) {
        // Aquí es donde simulamos el "React Router"
        if (ruta === '/' || ruta === '/Home/Index' || ruta === '/Home/Eventos') {
            await this.app.renderLista();
        }
        else if (ruta === '/Contacto') {
            this.app.renderConfig();
        }
        // Actualizamos la URL en el navegador sin recargar
        window.history.pushState({}, "", ruta);
    }
}

