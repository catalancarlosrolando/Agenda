using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    // Esta ruta cargará la página principal en el navegador
    [HttpGet("/")]
    public IActionResult Index()
    {
        return Redirect("/Index.html");
    }
}