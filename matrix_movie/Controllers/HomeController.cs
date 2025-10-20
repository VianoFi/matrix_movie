using Microsoft.AspNetCore.Mvc;
using MatrixMovie.Models;
using MatrixMovie.Data;
using Microsoft.EntityFrameworkCore;
using matrix_movie.Data;

namespace MatrixMovie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var films = await _context.Films.ToListAsync();
            return View(films);
        }

        [HttpPost]
        public async Task<IActionResult> AggiungiAlProfilo(int filmId)
        {
            // Qui aggiungerai la logica per associare il film al profilo utente
            // Per ora ritorniamo un JSON di successo
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RimuoviDalProfilo(int filmId)
        {
            // Qui aggiungerai la logica per rimuovere il film dal profilo utente
            return Json(new { success = true });
        }
    }
}