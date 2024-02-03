using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlatformaStreamingowa.Models;
using CustomIdentity.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace PlatformaStreamingowa.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Main", "Home");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Main()
        {
            var filmy = _context.Filmy.ToList();
            var kategorie = _context.Kategorie.ToList();
            var model = new Tuple<IEnumerable<Film>, IEnumerable<Kategoria>>(filmy, kategorie);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SetHttpOnlyCookie()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
            };

            Response.Cookies.Append("NazwaCiasteczka", "WartoœæCiasteczka", cookieOptions);
            return RedirectToAction("Index");
        }

        public IActionResult Search(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return RedirectToAction("Index");
            }

            var filmy = _context.Filmy.Where(f => f.Tytul.Contains(searchQuery)).ToList();

            if (!filmy.Any())
            {
                return RedirectToAction("Main");
            }

            if (filmy.Count == 1)
            {
                var film = filmy.First();

                if (User.IsInRole("User") && film.IsPremium)
                {
                    return RedirectToAction("Main");
                }

                return RedirectToAction("Watching", "Account", new { filmId = film.Id });
            }

            return RedirectToAction("Main");
        }

        public IActionResult NotFound()
        {
            return View(); 
        }
    }
}
