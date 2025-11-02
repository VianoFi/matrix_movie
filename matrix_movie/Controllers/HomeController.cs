using matrix_movie.Data;
using matrix_movie.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace matrix_movie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // ✅ INDEX: carica film + generi
        public IActionResult Index()
        {
            CaricaGeneri();

            var movies = _context.Movies.ToList();

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.WatchedMovies = _context.UserMovies
                    .Where(um => um.UserId == userId)
                    .Select(um => um.MovieId)
                    .ToList();
            }
            else
            {
                ViewBag.WatchedMovies = new List<int>();
            }

            ViewBag.IsSearch = false;
            ViewBag.SearchQuery = "";
            return View(movies);
        }

        // ✅ SEARCH: gestisce ricerca + filtro + generi dinamici
        [HttpGet]
        public IActionResult Search(string? query, string? genre = "Tutti")
        {
            CaricaGeneri();

            var moviesQ = _context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim();
                moviesQ = moviesQ.Where(m =>
                    m.Title.Contains(q) ||
                    m.Genre.Contains(q) ||
                    (m.Description ?? "").Contains(q) ||
                    (m.Plot ?? "").Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(genre) && genre != "Tutti")
                moviesQ = moviesQ.Where(m => m.Genre == genre);

            var movies = moviesQ.ToList();

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.WatchedMovies = _context.UserMovies
                    .Where(um => um.UserId == userId)
                    .Select(um => um.MovieId)
                    .ToList();
            }
            else
            {
                ViewBag.WatchedMovies = new List<int>();
            }

            ViewBag.IsSearch = true;
            ViewBag.SearchQuery = query ?? "";
            return View("Index", movies);
        }

        // ✅ Caricamento dinamico dei generi dal DB
        private void CaricaGeneri()
        {
            var generi = _context.Movies
                .Select(m => m.Genre)
                .Where(g => g != null && g.Trim() != "")
                .Distinct()
                .OrderBy(g => g)
                .ToList();

            ViewBag.Generi = generi;
        }

        public IActionResult Dettagli(int id)
        {
            CaricaGeneri();

            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null) return NotFound();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.IsWatched = _context.UserMovies.Any(um => um.UserId == userId && um.MovieId == id);
            }
            else ViewBag.IsWatched = false;

            return View(movie);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminaVisto(int id)
        {
            try
            {
                // Ottieni l'ID GUID dell'utente autenticato
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Utente non autenticato durante l'eliminazione.");
                    return RedirectToAction("Visti");
                }

                // Trova il record corrispondente
                var visto = _context.UserMovies
                    .FirstOrDefault(um => um.MovieId == id && um.UserId == userId);

                if (visto == null)
                {
                    _logger.LogWarning($"Nessun record trovato per UserId={userId} e MovieId={id}");
                    return RedirectToAction("Visti");
                }

                // Elimina il record e salva
                _context.UserMovies.Remove(visto);
                _context.SaveChanges();

                _logger.LogInformation($"Film con ID {id} eliminato dai visti per utente {userId}.");
                return RedirectToAction("Visti");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del film visto.");
                return RedirectToAction("Visti");
            }
        }




        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SegnaComeVisto(int id, DateTime? watchDate, string? comment)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Utente non autenticato" });

            if (!_context.UserMovies.Any(x => x.UserId == userId && x.MovieId == id))
            {
                _context.UserMovies.Add(new UserMovie
                {
                    UserId = userId,
                    MovieId = id,
                    WatchDate = watchDate ?? DateTime.Now,
                    Comment = comment
                });
                _context.SaveChanges();
            }

            return Json(new { success = true });
        }

        [Authorize]
        public IActionResult Visti()
        {
            CaricaGeneri();

            var userId = _userManager.GetUserId(User);
            var list = _context.UserMovies
                .Include(um => um.Movie)
                .Where(um => um.UserId == userId)
                .OrderByDescending(um => um.WatchDate)
                .ToList();
            return View(list);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }




}
