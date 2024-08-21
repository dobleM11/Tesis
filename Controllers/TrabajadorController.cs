using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Tesis.Models;
using Tesis.ViewModel;
using static System.Collections.Specialized.BitVector32;

namespace Tesis.Controllers {
    [Authorize]
    public class TrabajadorController : Controller {
        private readonly AppDbContext _context;

        public TrabajadorController(AppDbContext context) {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public IActionResult dashboard() {
            int seccion = int.Parse(User.Claims.LastOrDefault().Value);
            List<Sugerencia> sugerencias = _context.Sugerencias.Include(s => s.Seccion).Where(s => s.SeccionId == seccion || s.SeccionId == 3).ToList();
            Seccion seccion1 = _context.Secciones.Where(s => s.Id == seccion).FirstOrDefault();
            DashboardViewModel dashboardViewModel = new DashboardViewModel() {
                Seccion = seccion1,
                sugerencias = sugerencias
            };
            return View(dashboardViewModel);
        }

        [HttpGet]
        public ActionResult getListaPorFecha(DateTime fecha) {
            int seccion = int.Parse(User.Claims.LastOrDefault().Value);
            List<Turno> datos1 = _context.Turnos.Include(e => e.Usuario).Include(e => e.Tramite).Where(t => t.Fecha.Date == fecha.Date && t.Tramite.SeccionId == seccion).ToList();

            List<TramitesViewModel> datos = new List<TramitesViewModel>();
            foreach(var t in datos1) {
                datos.Add(new TramitesViewModel() {
                    id = t.Id,
                    Hora = t.Hora.ToString(),
                    Nombre = t.Usuario.Nombre.ToString(),
                    Rut = t.Usuario.Run.ToString(),
                    Tramite = t.Tramite.Nombre,
                    asistido = t.Asistencia
                });
            }

            return Json(datos);
        }


        // Método para registrar que un usuario asistió
        [HttpGet]
        public async Task<IActionResult> UsuarioAsistio(int id) {
            Turno turno = _context.Turnos.Include(e => e.Usuario).Where(t => t.Id == id).FirstOrDefault();
            TimeSpan horaEntrada = TimeSpan.Parse(DateTime.Now.ToString("h:mm:ss"));
            turno.Asistencia = 2;
            turno.HoradeEntrada = horaEntrada;
            _context.Turnos.Update(turno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(dashboard));
        }

        // Método para registrar que un usuario faltó
        [HttpGet]
        public async Task<IActionResult> UsuarioFalto(int id) {
            Turno turno = _context.Turnos.Include(e => e.Usuario).Where(t => t.Id == id).FirstOrDefault();
            var u = turno.Usuario;
            Usuario user = _context.Usuarios.Where(us => us.Run == u.Run).FirstOrDefault();
            user.Faltas += 1;
            turno.Asistencia = 1;
            _context.Usuarios.Update(user);
            _context.Turnos.Update(turno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(dashboard));
        }

        [HttpGet]
        public ActionResult getListaEstadisticaPorFecha(DateTime fecha) {
            int seccion = int.Parse(User.Claims.LastOrDefault().Value);
            List<Turno> datos1 = _context.Turnos
                .Include(e => e.Usuario)
                .Include(e => e.Tramite)
                .Where(t => t.Fecha.Month == fecha.Month && t.Fecha.Year == fecha.Year)
                .Where(t => t.Tramite.SeccionId == seccion)
                .ToList();

            // Crear un diccionario para almacenar los conteos por día del mes
            Dictionary<int, int> turnosPorDia = new Dictionary<int, int>();

            // Inicializar el diccionario con 0 para todos los días del mes
            int diasEnMes = DateTime.DaysInMonth(fecha.Year, fecha.Month);
            for(int dia = 1; dia <= diasEnMes; dia++) {
                turnosPorDia[dia] = 0;
            }

            // Llenar el diccionario con los conteos reales
            foreach(var turno in datos1) {
                int diaDelMes = turno.Fecha.Day;
                if(turnosPorDia.ContainsKey(diaDelMes)) {
                    turnosPorDia[diaDelMes]++;
                }
            }

            // Convertir el diccionario a una lista de objetos anónimos para JSON
            var turnosPorDiaList = turnosPorDia
                .Select(kv => new { Fecha = kv.Key, Count = kv.Value })
                .OrderBy(g => g.Fecha) // Ordenar por fecha ascendente
                .ToList();

            //foreach(var grupo in turnosPorDiaList) {
            //    Console.WriteLine($"{grupo.Fecha},{grupo.Count}");
            //}

            return Json(turnosPorDiaList);
        }
        public ActionResult DescargarExcel() {
            List<Sugerencia> sugerencias = _context.Sugerencias.Include(s => s.Seccion).ToList();
            var sugerenciasPorSeccion = sugerencias.GroupBy(s => s.Seccion).Select(g => new {
                Seccion = g.Key,
                Conteo = g.Count(),
                NombreSeccion = g.First().Seccion.Nombre
            }).OrderByDescending(g => g.Conteo).ToList();

            using(var package = new ExcelPackage()) {
                // Añadir una hoja de trabajo para las estadísticas generales
                var worksheetEstadisticas = package.Workbook.Worksheets.Add("Estadisticas");

                // Añadir contenido a la hoja de estadísticas
                worksheetEstadisticas.Cells[1, 1].Value = "Sección";
                worksheetEstadisticas.Cells[1, 2].Value = "Cantidad de reclamos y sugerencias";

                int row = 2;
                foreach(var grupo in sugerenciasPorSeccion) {
                    worksheetEstadisticas.Cells[row, 1].Value = grupo.NombreSeccion;
                    worksheetEstadisticas.Cells[row, 2].Value = grupo.Conteo;
                    row++;
                }

                // Ajustar el ancho de las columnas en la hoja de estadísticas
                worksheetEstadisticas.Cells.AutoFitColumns();

                // Crear una hoja por cada sección y agregar las sugerencias correspondientes
                foreach(var grupo in sugerenciasPorSeccion) {
                    var worksheet = package.Workbook.Worksheets.Add(grupo.NombreSeccion);

                    // Añadir encabezados a la hoja de trabajo de cada sección
                    worksheet.Cells[1, 1].Value = "Hora y fecha";
                    worksheet.Cells[1, 2].Value = "Tipo de sugerencia";
                    worksheet.Cells[1, 3].Value = "Texto";

                    // Agregar las sugerencias correspondientes a la sección
                    var sugerenciasDeSeccion = sugerencias.Where(s => s.Seccion.Id == grupo.Seccion.Id).ToList();
                    int rowSeccion = 2;
                    foreach(var sugerencia in sugerenciasDeSeccion) {
                        worksheet.Cells[rowSeccion, 1].Value = sugerencia.FechaHora.ToString("dd/MM/yyyy HH:mm");
                        worksheet.Cells[rowSeccion, 2].Value = sugerencia.TipoSugerencia;
                        worksheet.Cells[rowSeccion, 3].Value = sugerencia.Texto;
                        rowSeccion++;
                    }

                    // Ajustar el ancho de las columnas en la hoja de la sección
                    worksheet.Cells.AutoFitColumns();
                }

                // Convertir el archivo a un array de bytes
                var stream = new MemoryStream();
                package.SaveAs(stream);
                byte[] bytes = stream.ToArray();

                // Devolver el archivo Excel como una descarga
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");
            }
        }



    }
}
