using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tesis.Models;
using Tesis.ViewModel;

namespace Tesis.Controllers {
    public class TurnosController : Controller {
        private readonly AppDbContext _context;

        public TurnosController(AppDbContext context) {
            _context = context;
        }

        public IActionResult MiUsuario() {
            var u = GetUsuarioActual();
            Empleado e = _context.Empleados.Include(e => e.Rol).Include(e => e.Seccion).FirstOrDefault(e => e.Run.Equals(u.Run));
            List<Turno> turnos = _context.Turnos.Where(t => t.UsuarioRun.Equals(u.Run)).ToList();
            EmpleadoTurnosViewModel etvm = new EmpleadoTurnosViewModel() {
                Empleado = e,
                Turnos = turnos
            };
            return View(etvm);
        }
        [HttpGet]
        public IActionResult AgendarHoraGeneral() {
            var u = GetUsuarioActual();
            List<Turno> turnos = _context.Turnos.Where(t => t.UsuarioRun.Equals(u.Run)).ToList();
            List<Seccion> secciones = _context.Secciones.ToList();
            TurnosSeccionesViewModel tsvm = new TurnosSeccionesViewModel();
            tsvm.Secciones = secciones;
            tsvm.Turnos = turnos;
            tsvm.Turno = new Turno();
            return View(tsvm);
        }

        [HttpPost]
        public async Task<IActionResult> AgendarHoraGeneral(TurnosSeccionesViewModel tsvm) {
            var u = GetUsuarioActual();
            List<Turno> turnosUsuario = _context.Turnos.Where(t => t.UsuarioRun.Equals(u.Run)).ToList();
            List<Turno> turnosGeneral = _context.Turnos.Where(t => t.FechaHora == tsvm.Turno.FechaHora && t.Seccion == tsvm.Turno.Seccion).ToList();

            List<Turno> turnos = _context.Turnos.Where(t => t.UsuarioRun.Equals(u.Run)).ToList();
            List<Seccion> secciones = _context.Secciones.ToList();
            tsvm.Secciones = secciones;
            tsvm.Turnos = turnos;

            if(DateTime.Compare(tsvm.Turno.FechaHora, DateTime.Now) <= 0 || tsvm.Turno.FechaHora.DayOfWeek == DayOfWeek.Sunday || tsvm.Turno.FechaHora.DayOfWeek == DayOfWeek.Saturday) {
                ModelState.AddModelError("", "Debe ingresar una hora y fecha validas");
                return View(tsvm);
            } else {
                if(turnosGeneral != null) {
                    ModelState.AddModelError("", "Ese horario ya está ocupado");
                    return View(tsvm);
                } else {
                    bool horaActiva = false;
                    foreach(var item in turnosUsuario) {
                        if(item.SeccionId == tsvm.Turno.SeccionId && DateTime.Compare(tsvm.Turno.FechaHora, DateTime.Now) > 0) {
                            horaActiva = true;
                        }
                    }
                    if(horaActiva) {
                        ModelState.AddModelError("", "Ya posee una Hora en espera para esa sección");
                        return View(tsvm);
                    } else {
                        _context.Turnos.Add(
                        new Turno() {
                            UsuarioRun = u.Run,
                            FechaHora = tsvm.Turno.FechaHora,
                            SeccionId = tsvm.Turno.SeccionId
                        }
                                    );
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(MiUsuario));

                    }
                }
            }
        }

        private Usuario GetUsuarioActual() {
            Usuario usuario = _context.Usuarios.Include(u => u.Rol).FirstOrDefault(u => u.Run.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return usuario;
        }
    }
}
