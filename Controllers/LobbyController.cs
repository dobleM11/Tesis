using Microsoft.AspNetCore.Mvc;

namespace Tesis.Controllers {
    public class LobbyController : Controller {
        public IActionResult Inicio() {
            return View();
        }
    }
}
