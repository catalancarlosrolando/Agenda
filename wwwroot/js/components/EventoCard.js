export class EventoCard {
    static render(ev) {
        //const fechaFormat = new Date(ev.fechaHora).toLocaleString();
        const fecha = new Date(ev.fechaHora);
        const opciones = { day: 'numeric', month: 'long', hour: '2-digit', hour12: true };
        const fechaFormat = fecha.toLocaleString('es-ES', opciones);

        const prioridadClass = ev.prioridad === 3 ? 'danger' : (ev.prioridad === 2 ? 'warning' : 'info');
        return `
            <div class="col-md-4 mb-3">
               <div class="card border-${prioridadClass} shadow-sm">
                                        <div class="card-body">
                                            <div class="d-flex align-items-center mb-3 gap-2 justify-content-between">
                                            <h5 class="card-title">${ev.titulo}</h5>
                                           ${ev.alertaConfirmada ? `
                                                <button 
                                                    type="button" 
                                                    class="btn btn-sm btn-outline-success border-2 rounded-circle p-2 d-inline-flex align-items-center justify-content-center"
                                                    data-id="${ev.id}"
                                                    data-accion="confirmar"
                                                    data-estado="true"
                                                    title="Alerta Confirmada"
                                                    style="width: 34px; height: 34px; background-color: rgba(22, 145, 88, 0.1);"
                                                >
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-check-icon lucide-check"><path d="M20 6 9 17l-5-5"/></svg>
                                                </button>
                                            ` : `
                                                <button
                                                    onclick="app.confirmarAlerta(${ev.id}, true, this)" 
                                                    type="button" 
                                                    class="btn btn-sm btn-outline-secondary border-2 rounded-circle p-2 d-inline-flex align-items-center justify-content-center btn-alerta-pendiente"
                                                    data-id="${ev.id}"
                                                    data-accion="confirmar"
                                                    data-estado="false"
                                                    title="Haga clic para confirmar lectura"
                                                    style="width: 34px; height: 34px;"
                                                >
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-check-icon lucide-check"><path d="M20 6 9 17l-5-5"/></svg>
                                                </button>
                                            `}
                                            </div>


                                            <div class="d-flex align-items-center mb-3 mt-3 gap-2">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-calendar-icon lucide-calendar"><path d="M8 2v4"/><path d="M16 2v4"/><rect width="18" height="18" x="3" y="4" rx="2"/><path d="M3 10h18"/></svg>
                                                <p class="card-text text-muted">${fechaFormat}</p>
                                            </div>

                                            <span class="badge bg-${prioridadClass}">${ev.tipo || 'Evento'}</span>
                                            <button onclick="app.eliminarEvento(${ev.id})" class="btn btn-sm btn-outline-danger float-end ms-1">Borrar</button>
                                            <button onclick="app.actualizarEvento(${ev.id})" class="btn btn-sm btn-outline-danger float-end">Modificar</button>
                                        </div>
                                    </div>
                                </div>                     
        `;
    }
}