using Agenda;
using Microsoft.EntityFrameworkCore;
using AgendaApi.Data;
using Scalar.AspNetCore;
using FluentValidation;
using MediatR;
using AgendaApi.Behaviors;
using AgendaApi.Exceptions;

var builder = WebApplication.CreateBuilder(args);


// Configuración global de manejo de excepciones para devolver respuestas HTTP estandarizadas
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // Requerido para mapear errores estándar

// Add services to the container.

builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Configuración para SQLite persistente
builder.Services.AddDbContext<AgendaContext>(options =>
    options.UseSqlite("Data Source=agenda.db"));

builder.Services.AddOpenApi();

// Esto busca automáticamente CUALQUIER clase que herede de AbstractValidator dentro de tu proyecto y la registra
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Registrar MediatR buscando todos los Handlers en el proyecto
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);

    // REGISTRAR EL PIPELINE DE VALIDACIÓN
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});


var app = builder.Build();

app.UseExceptionHandler();

// Permite servir archivos estáticos (CSS, JS) desde la carpeta wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

// Mapea tanto la API como las Vistas
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Configure the HTTP request pipeline.
//swagger 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

// Reload page on refresh for SPA routes
app.MapFallbackToFile("Index.html");

app.Run();
