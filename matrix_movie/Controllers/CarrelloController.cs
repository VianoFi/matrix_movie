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
        private readonly ILogger<CarrelloController> _logger;
        private const decimal PrezzoUnitario = 7.50m;

        public CarrelloController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<CarrelloController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Mostra il carrello
        public IActionResult Index()
        {
            var carrello = HttpContext.Session.GetObjectFromJson<List<int>>("Carrello") ?? new List<int>();
            var films = _context.Movies.Where(m => carrello.Contains(m.Id)).ToList();
            ViewBag.Totale = films.Count * PrezzoUnitario;
            ViewBag.Count = carrello.Count;
            return View(films);
        }

        // POST: Aggiungi film al carrello
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Aggiungi(int id)
        {
            try
            {
                // Validazione ID
                if (id <= 0)
                {
                    _logger.LogWarning($"Tentativo di aggiungere film con ID non valido: {id}");
                    return Json(new { success = false, message = "ID non valido" });
                }

                // Verifica che il film esista
                if (!_context.Movies.Any(m => m.Id == id))
                {
                    _logger.LogWarning($"Tentativo di aggiungere film inesistente: ID {id}");
                    return Json(new { success = false, message = "Film non trovato" });
                }

                // Recupera o inizializza il carrello
                var carrello = HttpContext.Session.GetObjectFromJson<List<int>>("Carrello") ?? new List<int>();

                // Aggiungi solo se non presente
                if (!carrello.Contains(id))
                {
                    carrello.Add(id);
                    HttpContext.Session.SetObjectAsJson("Carrello", carrello);
                    _logger.LogInformation($"Film {id} aggiunto al carrello");
                }

                return Json(new { success = true, count = carrello.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiunta al carrello");
                return Json(new { success = false, message = "Errore durante l'aggiunta" });
            }
        }

        // POST: Rimuovi film dal carrello
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Rimuovi(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning($"Tentativo di rimuovere film con ID non valido: {id}");
                    return RedirectToAction(nameof(Index));
                }

                var carrello = HttpContext.Session.GetObjectFromJson<List<int>>("Carrello") ?? new List<int>();

                if (carrello.Remove(id))
                {
                    HttpContext.Session.SetObjectAsJson("Carrello", carrello);
                    _logger.LogInformation($"Film {id} rimosso dal carrello");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la rimozione dal carrello");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Svuota carrello
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Svuota()
        {
            try
            {
                HttpContext.Session.Remove("Carrello");
                _logger.LogInformation("Carrello svuotato");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante lo svuotamento del carrello");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Checkout con Stripe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout()
        {
            try
            {
                var carrello = HttpContext.Session.GetObjectFromJson<List<int>>("Carrello") ?? new List<int>();

                // Validazione: carrello vuoto
                if (carrello.Count == 0)
                {
                    _logger.LogWarning("Tentativo di checkout con carrello vuoto");
                    TempData["ErrorMessage"] = "Il carrello è vuoto";
                    return RedirectToAction(nameof(Index));
                }

                // Validazione: limite massimo articoli
                if (carrello.Count > 50)
                {
                    _logger.LogWarning($"Tentativo di checkout con troppi articoli: {carrello.Count}");
                    TempData["ErrorMessage"] = "Troppi articoli nel carrello";
                    return RedirectToAction(nameof(Index));
                }

                var films = _context.Movies.Where(m => carrello.Contains(m.Id)).ToList();

                // Verifica che tutti i film esistano ancora
                if (films.Count != carrello.Count)
                {
                    _logger.LogWarning("Alcuni film nel carrello non esistono più");
                    TempData["ErrorMessage"] = "Alcuni film non sono più disponibili";
                    return RedirectToAction(nameof(Index));
                }

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
                                Name = f.Title,
                                Description = f.Description ?? "",
                                Images = new List<string> { f.ImageUrl }
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

                _logger.LogInformation($"Sessione Stripe creata per {films.Count} articoli");

                return Redirect(session.Url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il checkout");
                TempData["ErrorMessage"] = "Errore durante il pagamento. Riprova più tardi.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Conferma pagamento
        public IActionResult Conferma()
        {
            HttpContext.Session.Remove("Carrello");
            _logger.LogInformation("Pagamento confermato, carrello svuotato");
            return View();
        }
    }
}