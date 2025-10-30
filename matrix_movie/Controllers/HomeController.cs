using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    // Pagina pubblica - nessuna autorizzazione
    public IActionResult Index()
    {
        return View();
    }

    // Solo utenti autenticati
    [Authorize]
    public IActionResult Profile()
    {
        return View();
    }

    // Solo Admin
    [Authorize(Roles = "Admin")]
    public IActionResult AdminPanel()
    {
        return View();
    }
}