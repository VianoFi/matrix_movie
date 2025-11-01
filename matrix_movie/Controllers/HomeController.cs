using matrix_movie.Data;
using matrix_movie.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

        // -------- HOME (griglia + barra a scorrimento) --------
        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = _userManager.GetUserId(User);
                var watchedIds = _context.UserMovies
                    .Where(um => um.UserId == userId)
                    .Select(um => um.MovieId)
                    .ToList();

                ViewBag.WatchedMovies = watchedIds;
            }
            else
            {
                ViewBag.WatchedMovies = new List<int>();
            }

            return View(movies);
        }

        // -------- DETTAGLI (blocca aggiunta se già visto) --------
        public IActionResult Dettagli(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound();

            //  verifica se l'utente loggato ha già visto il film
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                bool isWatched = _context.UserMovies.Any(um => um.UserId == userId && um.MovieId == id);
                ViewBag.IsWatched = isWatched;
            }
            else
            {
                ViewBag.IsWatched = false;
            }

            return View(movie);
        }


        // -------- SEGNARE COME VISTO --------
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SegnaComeVisto(int id, DateTime? watchDate, string? comment)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "Utente non autenticato" });

                var existing = _context.UserMovies.FirstOrDefault(x => x.UserId == userId && x.MovieId == id);
                if (existing == null)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel salvataggio del film nei visti");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------- LISTA VISTI (ord. cronologico, elimina) --------
        [Authorize]
        public IActionResult Visti()
        {
            var userId = _userManager.GetUserId(User);

            var userMovies = _context.UserMovies
                .Include(um => um.Movie)
                .Where(um => um.UserId == userId)
                .OrderByDescending(um => um.WatchDate)
                .ToList();

            return View(userMovies);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminaVisto(int id)
        {
            var userId = _userManager.GetUserId(User);
            var record = _context.UserMovies.FirstOrDefault(um => um.Id == id && um.UserId == userId);

            if (record != null)
            {
                _context.UserMovies.Remove(record);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Visti));
        }
    }
}
