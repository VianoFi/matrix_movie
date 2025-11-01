using matrix_movie.Data;
using matrix_movie.Helpers;
using matrix_movie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace matrix_movie.Controllers
{
    [Authorize]
    public class CarrelloController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private const decimal PrezzoUnitario = 7.50m;

        public CarrelloController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var carrello = HttpContext.Session.GetObjectFromJson<List<int>>("Carrello") ?? new List<int>();
            var films = _context.Movies.Where(m => carrello.Contains(m.Id)).ToList();
            ViewBag.Totale = films.Count * PrezzoUnitario;
            ViewBag.Count = carrello.Count;
            return View(films);
        }

        [HttpPost]
        public IActionResult Aggiungi(int id)
        {
            var carrello = HttpContext.Session.GetObjectFromJson<List<int>>("Carrello") ?? new List<int>();
            if (!carrello.Contains(id))
                carrello.Add(id);
            HttpContext.Session.SetObjectAsJson("Carrello", carrello);
            return Json(new { success = true, count = carrello.Count });
        }

        [HttpPost]
        public IActionResult Rimuovi(int id)
        {
            var carrello = HttpContext.Session.GetObjectFromJson<List<int>>("Carrello") ?? new List<int>();
            carrello.Remove(id);
            HttpContext.Session.SetObjectAsJson("Carrello", carrello);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Svuota()
        {
            HttpContext.Session.Remove("Carrello");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            var carrello = HttpContext.Session.GetObjectFromJson<List<int>>("Carrello") ?? new List<int>();
            var films = _context.Movies.Where(m => carrello.Contains(m.Id)).ToList();
            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = films.Select(f => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = PrezzoUnitario * 100,
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = f.Title
                        }
                    },
                    Quantity = 1
                }).ToList(),
                Mode = "payment",
                SuccessUrl = $"{domain}/Carrello/Conferma",
                CancelUrl = $"{domain}/Carrello/Index"
            };

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }

        public IActionResult Conferma()
        {
            HttpContext.Session.Remove("Carrello");
            return View();
        }
    }
}
