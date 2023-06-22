using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Claims;
using Tesis.Models;
using Tesis.ViewModel;



namespace Tesis.Controllers {
    [Authorize]
    public class TurnosController : Controller {
        private readonly AppDbContext _context;

        public TurnosController(AppDbContext context) {
            _context = context;
        }

        public IActionResult MiUsuario() {
            var u = GetUsuarioActual();
            Empleado e = _context.Empleados.Include(e => e.Rol).Include(e => e.Seccion).FirstOrDefault(e => e.Run.Equals(u.Run));
            List<Turno> turnos = _context.Turnos.Include(e => e.Seccion).Where(t => t.UsuarioRun.Equals(u.Run)).ToList();
            EmpleadoTurnosViewModel etvm = new EmpleadoTurnosViewModel() {
                Empleado = e,
                Turnos = turnos
            };
            return View(etvm);
        }

        [HttpGet]
        public IActionResult AgendarHoraGeneral() {
            var u = GetUsuarioActual();
            if(u != null) {
                List<Turno> turnos = _context.Turnos.Where(t => t.UsuarioRun.Equals(u.Run)).ToList();
                List<Seccion> secciones = _context.Secciones.ToList();
                TurnosSeccionesViewModel tsvm = new TurnosSeccionesViewModel();
                tsvm.Secciones = secciones;
                tsvm.Turnos = turnos;
                tsvm.Turno = new Turno();
                return View(tsvm);
            } else {
                return RedirectToAction("LoginIn", "Auth");
            }

        }

        [HttpPost]
        public async Task<IActionResult> AgendarHoraGeneral(TurnosSeccionesViewModel tsvm) {
            var u = GetUsuarioActual();
            List<Turno> turnosUsuario = _context.Turnos.Where(t => t.UsuarioRun.Equals(u.Run)).ToList();
            List<Turno> turnosGeneral = _context.Turnos.Where(t => t.SeccionId == tsvm.Turno.SeccionId).ToList();

            List<Turno> turnos = _context.Turnos.Where(t => t.UsuarioRun.Equals(u.Run)).ToList();
            List<Seccion> secciones = _context.Secciones.ToList();
            tsvm.Secciones = secciones;
            tsvm.Turnos = turnos;


            // Obtener el año actual
            bool esFeriado = await FeriadoAsync(tsvm.Turno.FechaHora);

            bool ocupado = false;
            if(
            DateTime.Compare(tsvm.Turno.FechaHora, DateTime.Now) <= 0 || // se compara si la fecha y hora ingresada es despues de la fecha y hora actual
           tsvm.Turno.FechaHora.DayOfWeek == DayOfWeek.Sunday || // se verifica que la hora es domingo 
           tsvm.Turno.FechaHora.DayOfWeek == DayOfWeek.Saturday || // se verifica que la hora es sabado
           tsvm.Turno.FechaHora.Hour < 9 || // se verifica que la hora sea mayor o igual a las 9
           tsvm.Turno.FechaHora.Hour > 14 || // se verifica que la hora sea menor a las 14
           tsvm.Turno.FechaHora.Minute % 10 != 0 || // se verifica que se haya seleccionado un minuto multiplo de 10
           esFeriado
           ) {
                ModelState.AddModelError("", "Debe ingresar una hora y fecha validas");
                return View(tsvm);
            } else {
                if(turnosGeneral.Any(item => DateTime.Compare(item.FechaHora, tsvm.Turno.FechaHora) == 0)) {
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

        private async Task<bool> FeriadoAsync(DateTime fechaHora) {
            bool esFeriado = false;
            // Crea una instancia del servicio de feriados de Nager.Date para Chile
            string apiUrl = $"https://apis.digital.gob.cl/fl/feriados{DateTime.Now.Year}";
            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if(response.IsSuccessStatusCode) {
                    // Leer la respuesta JSON
                    string json = await response.Content.ReadAsStringAsync();

                    // Analizar el JSON y obtener los feriados
                    var feriados = Newtonsoft.Json.JsonConvert.DeserializeObject<Feriado[]>(json);

                    // Verificar si t.FechaHora es un feriado
                    esFeriado = feriados.Any(feriado => feriado.Fecha.Date.Day == fechaHora.Day && feriado.Fecha.Date.Month == fechaHora.Month);

                }
            }
            return esFeriado;
        }

        [HttpGet]
        public IActionResult EditarHora(int turnoId) {
            Turno U = _context.Turnos.Include(e => e.Seccion).FirstOrDefault(u => u.Id == turnoId);
            return View(U);
        }

        [HttpPost]
        public async Task<IActionResult> EditarHora(Turno t) {

            bool esFeriado = await FeriadoAsync(t.FechaHora);// se comprueba si el día seleccionado es feriado

            var user = GetUsuarioActual();
            var U = _context.Turnos.Include(e => e.Seccion).FirstOrDefault(u => u.Id == t.Id);
            List<Turno> turnosUsuario = _context.Turnos.Where(t => t.UsuarioRun.Equals(user.Run)).ToList();
            List<Turno> turnosGeneral = _context.Turnos.Where(x => x.FechaHora == t.FechaHora && x.Seccion == t.Seccion).ToList();

            if(
                DateTime.Compare(t.FechaHora, DateTime.Now) <= 0 || // se compara si la fecha y hora ingresada es despues de la fecha y hora actual
                t.FechaHora.DayOfWeek == DayOfWeek.Sunday || // se verifica que la hora es domingo 
                t.FechaHora.DayOfWeek == DayOfWeek.Saturday || // se verifica que la hora es sabado
                t.FechaHora.Hour < 9 || // se verifica que la hora sea mayor o igual a las 9
                t.FechaHora.Hour > 14 || // se verifica que la hora sea menor a las 14
                t.FechaHora.Minute % 30 != 0 || // se verifica que se haya seleccionado un minuto multiplo de 30
                esFeriado
                ) {
                ModelState.AddModelError("", "Debe ingresar una hora y fecha validas"); // da error si alguna de las anteriores verificaciones se cumple
                return View(U);
            } else {
                if(turnosGeneral.Count != 0) {
                    ModelState.AddModelError("", "Ese horario ya está ocupado");
                    return View(U);
                } else {
                    U.FechaHora = t.FechaHora;
                    _context.Turnos.Update(U);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MiUsuario));
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> EliminarHora(int turnoId) {
            var U = _context.Turnos.FirstOrDefault(u => u.Id == turnoId);
            if(U == null) {
                return NotFound();
            } else {
                _context.Remove(U);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MiUsuario));
        }

        private Usuario GetUsuarioActual() {
            Usuario usuario = _context.Usuarios.Include(u => u.Rol).FirstOrDefault(u => u.Run.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return usuario;
        }
    }
}
public class Feriado {
    public DateTime Fecha { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public string Irrenunciable { get; set; }
}