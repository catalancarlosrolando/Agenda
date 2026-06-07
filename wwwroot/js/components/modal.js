export class Modal {
    render(ev) {
        // Evitamos que falle el substring si la fecha viene vacía (al crear un evento nuevo)
        const fechaFormateada = ev.fechaHora ? ev.fechaHora.substring(0, 16) : '';

        return `
        <div class="modal fade" id="exampleModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">Modificar Evento</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <form id="form-modificar">
                        <div class="modal-body">
                            <input type="hidden" name="id" id="modificar-id">
                            
                            <label for="modificar-titulo">Titulo</label>
                            <input value="${ev.titulo || ''}" type="text" name="titulo" id="modificar-titulo" class="form-control mb-3" required>

                            <label for="modificar-fecha">Fecha Nueva</label>
                            <input value="${fechaFormateada}" type="datetime-local" name="fechaHora" id="modificar-fecha" class="form-control mb-3" required>

                            <label for="modificar-prioridad">Prioridad</label>
                            <select name="prioridad" id="modificar-prioridad" class="form-select mb-3">
                                <option value="1" ${ev.prioridad == 1 ? 'selected' : ''}>Baja</option>
                                <option value="2" ${ev.prioridad == 2 ? 'selected' : ''}>Media</option>
                                <option value="3" ${ev.prioridad == 3 ? 'selected' : ''}>Alta</option>
                            </select>

                            <label for="modificar-descripcion">Descripción</label>
                            <textarea name="descripcion" id="modificar-descripcion" class="form-control mb-3" rows="3">${ev.descripcion || ''}</textarea>

                            <label for="modificar-telefono">Teléfono (para alertas críticas por SMS)</label>
                            <input value="${ev.telefonoDestino || ''}" type="text" name="telefonoDestino" id="modificar-telefono" class="form-control mb-3" placeholder="+549264XXXXXXX" required>

                            <label for="modificar-mail">Email</label>
                            <input value="${ev.emailDestino || ''}" type="email" name="emailDestino" id="modificar-mail" class="form-control mb-3" placeholder="ingrese su mail para recibir alertas" required>  
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                            <button type="submit" class="btn btn-primary" id="button-send">Guardar cambios</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        `;
    }
}