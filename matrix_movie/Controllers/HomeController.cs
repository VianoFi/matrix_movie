using matrix_movie.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace matrix_movie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Dati fittizi - poi verranno dal database
            var movies = new List<Movie>
            {
                new Movie { Id = 1, Title = "The Matrix", ImageUrl = "https://image.tmdb.org/t/p/w500/f89U3ADr1oiB1s9GkdPOEpXUk5H.jpg", Genre = "Sci-Fi", Year = 1999, IsWatched = false },
                new Movie { Id = 2, Title = "Inception", ImageUrl = "https://image.tmdb.org/t/p/w500/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg", Genre = "Sci-Fi", Year = 2010, IsWatched = false },
                new Movie { Id = 3, Title = "Interstellar", ImageUrl = "https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg", Genre = "Sci-Fi", Year = 2014, IsWatched = false },
                new Movie { Id = 4, Title = "The Dark Knight", ImageUrl = "https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg", Genre = "Action", Year = 2008, IsWatched = false },
                new Movie { Id = 5, Title = "Pulp Fiction", ImageUrl = "https://image.tmdb.org/t/p/w500/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg", Genre = "Crime", Year = 1994, IsWatched = false },
                new Movie { Id = 6, Title = "Fight Club", ImageUrl = "https://image.tmdb.org/t/p/w500/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg", Genre = "Drama", Year = 1999, IsWatched = false },
                new Movie { Id = 7, Title = "Forrest Gump", ImageUrl = "https://image.tmdb.org/t/p/w500/arw2vcBveWOVZr6pxd9XTd1TdQa.jpg", Genre = "Drama", Year = 1994, IsWatched = false },
                new Movie { Id = 8, Title = "The Shawshank Redemption", ImageUrl = "https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg", Genre = "Drama", Year = 1994, IsWatched = false }
            };

            return View(movies);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}