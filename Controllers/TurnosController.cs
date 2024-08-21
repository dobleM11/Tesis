using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            Usuario e = _context.Usuarios.FirstOrDefault(e => e.Run.Equals(u.Run));

            // Obtén la fecha y hora actual
            DateTime ahora = DateTime.Now;

            // Filtra los turnos para que solo traiga los que sean después de la fecha y hora actual
            List<Turno> turnos = _context.Turnos
                .Include(t => t.Tramite.Seccion)
                .Where(t => t.UsuarioRun.Equals(u.Run) && t.Fecha >= ahora)
                .ToList();

            UsuarioTurnosViewModel etvm = new UsuarioTurnosViewModel() {
                Usuario = e,
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
                secciones.RemoveAt(secciones.Count - 1);
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
            List<Turno> turnosUsuario = _context.Turnos.Include(t=>t.Tramite).Where(t => t.UsuarioRun.Equals(u.Run)).ToList();
            List<Turno> turnosGeneral = _context.Turnos.Where(t => t.Tramite.SeccionId == tsvm.Turno.Tramite.SeccionId).ToList();

            List<Turno> turnos = _context.Turnos.Where(t => t.UsuarioRun.Equals(u.Run)).ToList();
            List<Seccion> secciones = _context.Secciones.ToList();
            secciones.RemoveAt(secciones.Count - 1);
            tsvm.Secciones = secciones;
            tsvm.Turnos = turnos;

            // Obtener el año actual
            bool esFeriado = await FeriadoAsync(new DateTime(tsvm.Turno.Fecha.Year, tsvm.Turno.Fecha.Month, tsvm.Turno.Fecha.Day));

            // Definir esFeriado antes de la condición
            bool esFeriadoIrrenunciable =
     (tsvm.Turno.Fecha.Month == 1 && tsvm.Turno.Fecha.Day == 1) || // Año Nuevo
     (tsvm.Turno.Fecha.Month == 5 && tsvm.Turno.Fecha.Day == 1) || // Día del Trabajador
     (tsvm.Turno.Fecha.Month == 9 && (tsvm.Turno.Fecha.Day == 18 || tsvm.Turno.Fecha.Day == 19)) || // Independencia Nacional y Glorias del Ejército
     (tsvm.Turno.Fecha.Month == 12 && (tsvm.Turno.Fecha.Day == 8 || tsvm.Turno.Fecha.Day == 25)); // Inmaculada Concepción y Navidad

            bool fechaHoraInvalida =
                DateTime.Compare(new DateTime(tsvm.Turno.Fecha.Year, tsvm.Turno.Fecha.Month, tsvm.Turno.Fecha.Day), DateTime.Now) < 0 ||
                tsvm.Turno.Fecha.DayOfWeek == DayOfWeek.Sunday ||
                tsvm.Turno.Fecha.DayOfWeek == DayOfWeek.Saturday ||
                esFeriado || esFeriadoIrrenunciable;

            if(fechaHoraInvalida) {
                ModelState.AddModelError("", "Debe ingresar una hora y fecha válidas");
                return View(tsvm);
            } else {
                if(turnosGeneral.Any(item => item.Fecha.Date == tsvm.Turno.Fecha.Date && item.Hora == tsvm.Turno.Hora)) {
                    ModelState.AddModelError("", "Ese horario ya está ocupado");
                    return View(tsvm);
                } else {
                    if(turnosUsuario.Any(item => item.Fecha.Date == tsvm.Turno.Fecha.Date && item.Hora == tsvm.Turno.Hora)) {
                        ModelState.AddModelError("", "Ya tiene un turno asignado para ese dia y esa hora");
                        return View(tsvm);
                    } else {
                        bool horaActiva = false;
                        foreach(var item in turnosUsuario) {
                            if(item.Tramite.SeccionId == tsvm.Turno.Tramite.SeccionId && DateTime.Compare(item.Fecha.Date, DateTime.Now) > 0) {
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
                                    Fecha = tsvm.Turno.Fecha,
                                    Hora = tsvm.Turno.Hora,
                                    TramiteId = tsvm.Turno.TramiteId,
                                    Asistencia=0
                                }
                            );
                            Tramite x = _context.Tramites.Where(e => e.Id==tsvm.Turno.TramiteId).FirstOrDefault();
                            x.Solicitudes += 1;
                            _context.Tramites.Update(x);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(MiUsuario));
                        }
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
            Turno U = _context.Turnos.Include(e => e.Tramite.Seccion).FirstOrDefault(u => u.Id == turnoId);
            return View(U);
        }

        [HttpPost]
        public async Task<IActionResult> EditarHora(Turno t) {
            bool esFeriado = await FeriadoAsync(new DateTime(t.Fecha.Year, t.Fecha.Month, t.Fecha.Day));

            var user = GetUsuarioActual();
            List<Turno> turnosUsuario = _context.Turnos.Where(t => t.UsuarioRun.Equals(user.Run)).ToList();
            var U = _context.Turnos.Include(e => e.Tramite.Seccion).FirstOrDefault(u => u.Id == t.Id);
            List<Turno> turnosGeneral = new List<Turno>();
            bool esFeriadoIrrenunciable =
    (t.Fecha.Month == 1 && t.Fecha.Day == 1) || // Año Nuevo
    (t.Fecha.Month == 5 && t.Fecha.Day == 1) || // Día del Trabajador
    (t.Fecha.Month == 9 && (t.Fecha.Day == 18 || t.Fecha.Day == 19)) || // Independencia Nacional y Glorias del Ejército
    (t.Fecha.Month == 12 && (t.Fecha.Day == 8 || t.Fecha.Day == 25)); // Inmaculada Concepción y Navidad

            if(
              DateTime.Compare(new DateTime(t.Fecha.Year, t.Fecha.Month, t.Fecha.Day), DateTime.Now) <= 0 ||
                                  t.Fecha.DayOfWeek == DayOfWeek.Sunday ||
                                   t.Fecha.DayOfWeek == DayOfWeek.Saturday ||
                                    esFeriado || esFeriadoIrrenunciable
            ) {
                ModelState.AddModelError("", "Debe ingresar una hora y fecha válidas"); // da error si alguna de las anteriores verificaciones se cumple
                return View(U);
            } else {
                if(turnosUsuario.Any(item => item.Fecha.Date == t.Fecha.Date && item.Hora == t.Hora)) {
                    ModelState.AddModelError("", "Ya tiene un turno asignado para ese dia y esa hora");
                    return View(U);
                } else {
                    if(turnosGeneral.Count != 0) {
                        ModelState.AddModelError("", "Ese horario ya está ocupado");
                        return View(U);
                    } else {
                        U.Fecha = t.Fecha;
                        U.Hora = t.Hora;
                        _context.Turnos.Update(U);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(MiUsuario));
                    }
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> EliminarHora(int turnoId) {
            var U = _context.Turnos.FirstOrDefault(u => u.Id == turnoId);


            if(U == null) {
                return NotFound();
            } else {
                var t = _context.Tramites.FirstOrDefault(x => x.Id == U.TramiteId);
                t.Solicitudes -= 1;
                _context.Tramites.Update(t);
                _context.Turnos.Remove(U);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MiUsuario));
        }

        private Usuario GetUsuarioActual() {
            Usuario usuario = _context.Usuarios.Include(u => u.Rol).FirstOrDefault(u => u.Run.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return usuario;
        }

        [HttpGet]
        public IActionResult ObtenerTramitesPorSeccion(int seccionId) {
            List<Tramite> tramites = _context.Tramites.Where(t => t.Seccion.Id == seccionId).ToList();
            return Json(tramites);
        }

        [HttpGet]
        public IActionResult ObtenerHorasOcupadasPorFechaYSeccion(DateTime fecha, int seccionId) {
            // Obtén las horas ocupadas en la fecha y sección específicas desde tu base de datos
            var horasOcupadas = _context.Turnos
                .Where(t => t.Tramite.SeccionId == seccionId && t.Fecha.Date == fecha.Date)
                .ToList();

            // Obtener solo las horas formateadas
            var horasOcupadasFormateadas = horasOcupadas.Select(t => t.Hora.ToString("hh\\:mm"));

            return Json(horasOcupadasFormateadas);
        }

        public IActionResult ObtenerDetalles(int id) {
           
            var detalle = new Dictionary<string, string>();

            switch(id) {
                case 1:
                    detalle["Edificio"] = "Edificio de la municipalidad";
                    detalle["Piso"] = "1";
                    detalle["Oficina"] = "101";
                    detalle["Documento1"] = "Copia de cedula de identidad del solicitante.";
                    detalle["Documento2"] = "Copia simple de dominio vigente.";
                    break;
                case 2:
                    detalle["Edificio"] = "Edificio de la municipalidad";
                    detalle["Piso"] = "1";
                    detalle["Oficina"] = "101";
                    detalle["Documento1"] = "Solicitud firmada por el propietario y arquitecto del proyecto.";
                    detalle["Documento2"] = "Fotocopia del certificado de informaciones previas.";
                    detalle["Documento3"] = "Formulario unico de estadisticas de edificación.";
                    break;
                case 3:
                    detalle["Edificio"] = "Edificio de la municipalidad";
                    detalle["Piso"] = "2";
                    detalle["Oficina"] = "202";
                    detalle["Documento1"] = "Fotocopia de cedula de identidad.";
                    detalle["Documento2"] = "Acreditación de ingresos familiares.";
                    detalle["Documento3"] = "Certificados medicos.";
                    break; 
                case 4:
                    detalle["Edificio"] = "Edificio de la municipalidad";
                    detalle["Piso"] = "2";
                    detalle["Oficina"] = "202";
                    detalle["Documento1"] = "Fotocopia de cedula de identidad del apoderado y estudiante.";
                    detalle["Documento2"] = "Acreditación de ingresos familiares.";
                    break;
                // Añade más casos según sea necesario
                default:
                    detalle["Edificio"] = "Edificio Desconocido";
                    detalle["Piso"] = "N/A";
                    detalle["Oficina"] = "N/A";
                    detalle["Documento1"] = "Documento Desconocido";
                    break;
            }

            return Json(detalle);
        }

        public IActionResult Historial() {
            var u = GetUsuarioActual();
            Usuario e = _context.Usuarios.FirstOrDefault(e => e.Run.Equals(u.Run));

            // Obtén la fecha y hora actual
            DateTime ahora = DateTime.Now;

            // Filtra los turnos para que solo traiga los que sean antes de la fecha y hora actual
            List<Turno> turnos = _context.Turnos
                .Include(t => t.Tramite.Seccion)
                .Where(t => t.UsuarioRun.Equals(u.Run) && t.Fecha < ahora)
                .ToList();

            UsuarioTurnosViewModel etvm = new UsuarioTurnosViewModel() {
                Usuario = e,
                Turnos = turnos
            };

            return View(etvm);
        }
    }
}
public class Feriado {
    public DateTime Fecha { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public string Irrenunciable { get; set; }
}