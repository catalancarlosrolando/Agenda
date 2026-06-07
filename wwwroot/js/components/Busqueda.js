export class Busqueda {
    static render() {
        return `
            <div class="container mt-5">
            <h1 class="text-center mb-4">Mi Agenda Online</h1>
                <div class="card shadow-sm mb-3">
                    <div class="card-body">
                        <h5 class="card-title">Buscar por titulo</h5>
                        <div class="row g-2 align-items-end">
                            <div class="col-md-6">
                                <label for="titulo" class="form-label mb-1">Titulo</label>
                                <div class="input-group input-group-sm">
                                  <input type="text" id="titulo" class="form-control" placeholder="Ej. Parcial">
                                  <button onclick="app.buscarEvento()" class="btn btn-primary">Buscar</button>
                                </div>
                            </div>
                            <div class="col-md-2">
                                <button onclick="app.guardarEvento()" class="btn btn-success btn-sm w-100">Nuevo Evento</button>
                            </div>
                        </div>
                    </div>
                </div> 
                <div class="d-flex gap-2 justify-content-end mb-3">
  <button onclick="app.renderLista()" class="btn btn-outline-primary btn-sm">Cargar eventos</button>
</div>
            </div>
        `;
    }
}