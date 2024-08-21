using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Tesis.Models;
using Tesis.ViewModel;

namespace Tesis.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<TrabajadorController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<TrabajadorController> logger, AppDbContext context) {
            _logger = logger;
            _context = context;
        }
        public IActionResult Inicio() {
            Usuario usuario = _context.Usuarios.Include(u => u.Rol).FirstOrDefault(u => u.Run.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            if(usuario == null) {
                usuario = new Usuario() {
                Faltas=0
                };
            }

            List<Tramite> t =_context.Tramites.ToList();

            t= t.OrderByDescending(z => z.Solicitudes).ToList();
            List<Tramite> tramites = new List<Tramite>();
            tramites.Add(t[0]);
            tramites.Add(t[1]);
            tramites.Add(t[2]);
            tramites.Add(t[3]);

            UsuarioTramitesViewModel ut = new UsuarioTramitesViewModel() { 
            Tramites = t,
            Usuario=usuario
            };
            return View(ut);
        }
        public IActionResult Secciones() {
            return View();
        }

        public IActionResult PreguntasFrecuentes() {
            return View();
        }
        public IActionResult Dirección_de_Obras_Municipales() {
            return View();
        }
        public IActionResult Dirección_de_Desarrollo_Comunitario() {
            return View();
        }

        public IActionResult Login() {
            return View();
        }
        [HttpGet]
        public IActionResult SugerenciasReclamos() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SugerenciasReclamos(Sugerencia s) {
            _context.Sugerencias.Add(
                        new Sugerencia() {
                            FechaHora = DateTime.Now,
                            Texto = s.Texto,
                            SeccionId = s.SeccionId,
                            TipoSugerencia = s.TipoSugerencia
                        });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Inicio));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}