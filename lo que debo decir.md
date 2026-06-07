.NET Core maneja la asincronía (async/await) de forma nativa a través del Thread Pool. Cuando un endpoint hace una consulta pesada a la base de datos de manera asíncrona, el hilo que atendía esa petición se libera para atender a otros usuarios, evitando el cuello de botella en el servidor web. Sin embargo, la base de datos sigue sufriendo, porque la asincronía en el backend no disminuye la cantidad de consultas que le llegan a la DB (como le pasó al creador de Coolify en el post que analizamos).


1-  creamos la webapi con el comando `dotnet new webapi -n AgendaApi` y luego agregamos los paquetes necesarios para trabajar con Entity Framework Core, MediatR y FluentValidation. Para esto, ejecutamos los siguientes comandos en la terminal:

```

2- instalamos MediatR y FluentValidation para implementar el patrón CQRS en nuestro proyecto. MediatR nos permite separar las responsabilidades de nuestras acciones (queries y commands) y FluentValidation nos ayuda a validar los datos de entrada de manera eficiente.

MediatR se instala con el siguiente comando:

```dotnet add package MediatR
```
FluentValidation se instala con el siguiente comando:

```dotnet add package FluentValidation
```

importamos los namespaces necesarios en nuestro archivo Program.cs para configurar los servicios de MediatR y FluentValidation. Esto se hace agregando las siguientes líneas al inicio del archivo:

```csharp
using FluentValidation;
using MediatR;
using AgendaApi.Behaviors;

creamos ValidationBehavior.cs para implementar un Pipeline Behavior que se encargue de validar las peticiones antes de que lleguen a los Handlers. Este middleware interceptará las peticiones y ejecutará las validaciones definidas en FluentValidation, deteniendo la ejecución si alguna validación falla y retornando un error 400 Bad Request.

```csharp