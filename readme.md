1. Creación del proyecto y paquetes
En lugar de instalar el paquete InMemory, instalaremos el de Sqlite y el de Design (que es el que permite crear las tablas físicamente).

Bash
# Crear el proyecto con controladores
dotnet new webapi --use-controllers -o AgendaApi
cd AgendaApi

# Agregar soporte para SQLite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# Agregar herramientas para las migraciones (crear la BD física)
dotnet add package Microsoft.EntityFrameworkCore.Design

# Abrir en VS Code ( Abre una nueva ventana de VS Code con el proyecto actual )
code -r .

2. Configuración en Program.cs

# En Program.cs, busca donde se agregan los servicios:
builder.Services.AddControllers();

# Configuración para SQLite persistente
builder.Services.AddDbContext<AgendaContext>(options =>
    options.UseSqlite("Data Source=agenda.db"));


3. Creación del contexto de datos
En la carpeta Data, crea el archivo AgendaContext.cs 

4. Creación del modelo de datos
En la carpeta Models, crea el archivo "mimodelo".cs

5. Creaciob de los controladores
En la carpeta Controllers, crea el archivo "micontrolador".cs

6. Migraciones y creación de la base de datos
En la terminal, ejecuta los siguientes comandos para crear la base de datos física: 
# Instalar la herramienta de Entity Framework (si no la tienes):

Bash
dotnet tool install --global dotnet-ef

# Crear la migración (esto genera el código para crear la tabla Eventos):

Bash
dotnet ef migrations add MigracionInicial

# Actualizar la base de datos (esto crea el archivo agenda.db):

Bash
dotnet ef database update

7. Ejecutar la aplicación
Finalmente, ejecuta la aplicación para asegurarte de que todo funciona correctamente:
Bash
dotnet run  

# Prueba con Postman o curl para crear un evento o
# ir a la URL de OpenAPI/Swagger que mencionamos antes (usualmente https://localhost:XXXX/swagger o la que te indique la consola) y podrás probar el POST enviando un JSON como este:
{
  "titulo": "Parcial de Programación",
  "descripcion": "Estudiar recursividad y arrays",
  "fechaHora": "2024-05-20T10:00:00",
  "tipo": "Examen",
  "prioridad": 3
}





interesante, y aqui puede entrar el tema de los componentes, llamando a otra clase que retorne contenido html:



async renderLista() {

        const eventos = await this.service.obtenerTodos();

        this.mainContainer.innerHTML = `

            <h2>Mis Eventos</h2>

            <div class="row" id="lista-eventos">

                ${eventos.map(e => `<li>${e.titulo}</li>`).join('')}

            </div>

        `;

    }



---

el contenido del innerHtml = ''

puedo retornarlo desde otra clase. 


----
toda la logica de negocios para el ABM de eventos, puede estar en una clase aparte, y el controlador solo se encarga de recibir las peticiones y llamar a esa clase.
configurar con una capa de datos.

decoradores [httpget, httppost, httpdelete, httpput] para cada metodo del controlador.

recordar utilizar las buenas practicas de api. verbos, sustantivos en la rutas, codigode estado, etc.