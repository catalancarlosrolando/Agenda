import { EventoCard } from '../components/EventoCard.js';
import { Busqueda } from '../components/Busqueda.js';

export class Home {
    render(eventos) {
        return `
            ${Busqueda.render()}
            <div class="row">
                ${eventos.map(e => EventoCard.render(e)).join('')}
            </div>  
        `;
    }
}