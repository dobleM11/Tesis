using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tesis.Models;

namespace Tesis.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context) {
            _logger = logger;
            _context = context;
        }
        public IActionResult Inicio() {
            return View();
        }
        public IActionResult Vista1() {
            return View();
        }
        public IActionResult Login() {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}