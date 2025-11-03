using matrix_movie.Data;
using matrix_movie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace matrix_movie.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📜 Lista film
        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View(movies);
        }

        // ➕ Aggiungi film
        public IActionResult AddMovie()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMovie(Movie movie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Movies.Add(movie);
                    _context.SaveChanges();

                    TempData["Success"] = "Film aggiunto con successo!";
                    return RedirectToAction(nameof(Index));
                }

                // Mostra errori di validazione (utile per debug)
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Errore ModelState: {error.ErrorMessage}");
                }

                TempData["Error"] = "Compila tutti i campi correttamente.";
                return View(movie);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Errore: {ex.Message}");
                TempData["Error"] = "Errore durante il salvataggio del film.";
                return View(movie);
            }
        }


        // ✏️ Modifica film
        public IActionResult EditMovie(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMovie(Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Update(movie);
                _context.SaveChanges();
                TempData["Success"] = "Film modificato con successo!";
                return RedirectToAction(nameof(Index));
            }

            // Se fallisce la validazione, mostra di nuovo il form
            return View(movie);
        }

        // ❌ Elimina film
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMovie(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                TempData["Success"] = "Film eliminato!";
            }
            return RedirectToAction(nameof(Index));
        }
    }

}
