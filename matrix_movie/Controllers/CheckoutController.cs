using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using matrix_movie.Data;
using matrix_movie.Helpers;

namespace matrix_movie.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const decimal PrezzoUnitario = 7.50m;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreaSessione()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<int>>("Carrello") ?? new List<int>();
            if (!cart.Any())
                return RedirectToAction("Index", "Carrello");

            var movies = _context.Movies.Where(m => cart.Contains(m.Id)).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = movies.Select(m => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = PrezzoUnitario * 100, // centesimi
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = m.Title,
                            Images = new List<string> { m.ImageUrl ?? "" }
                        }
                    },
                    Quantity = 1
                }).ToList(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Carrello/Index"
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        public IActionResult Success()
        {
            HttpContext.Session.Remove("Carrello");
            return View();
        }
    }
}
