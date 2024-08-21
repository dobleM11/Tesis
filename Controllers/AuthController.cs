using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using Tesis.Models;
using Tesis.ViewModel;

namespace Tesis.Controllers {
    public class AuthController : Controller {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> LoginIn() {
            var usuarios = _context.Usuarios.ToList(); // se obtienen todos los usuarios
            var empleados = _context.Empleados.ToList(); // se obtienen todos los empleados
            var roles = _context.Roles.ToList(); // se obtienen todos los roles 
            var secciones = _context.Secciones.ToList(); // se obtienen todos los roles 
            var tramites = _context.Tramites.ToList(); // se obtienen todos los roles 

            CratePassHash("123456", out byte[] passwordHash, out byte[] passwordSalt);

            if(roles.Count() == 0) { // se verifica si hay roles
                //si no los hay se crean
                _context.Roles.Add(
                    new Rol() {
                        Nombre = "Usuario",
                        Descripcion = "El usuario solo tíene acceso a su propia lista de horas pedidas por él."
                    }
                );

                _context.Roles.Add(
                    new Rol() {
                        Nombre = "Empleado",
                        Descripcion = "El empleado tiene acceso a todas las horas pedidas en la sección que tiene asignada."
                    }
                );

                _context.Roles.Add(
                    new Rol() {
                        Nombre = "JefeSeccion",
                        Descripcion = "El jefe de sección tiene acceso a todo lo relacionado con su sección."
                    }
                );

                _context.Roles.Add(
                    new Rol() {
                        Nombre = "Admin",
                        Descripcion = "El administrador tiene acceso a todas las secciones."
                    }
                );
            }
            await _context.SaveChangesAsync();

            if(secciones.Count() == 0) { // se verifica si hay secciones
                //si no los hay se crean
                _context.Secciones.Add(
                    new Seccion() {
                        Nombre = "Dirección de Obras Municipales",
                        Descripcion = "Dirección de Obras Municipales"
                    }
                );
                _context.Secciones.Add(
                    new Seccion() {
                        Nombre = "Dirección de Desarrollo Comunitario",
                        Descripcion = "Dirección de Desarrollo Comunitario"
                    }
                );

            }
            await _context.SaveChangesAsync();

            if(tramites.Count() == 0) { // se verifica si hay secciones
                //si no los hay se crean
                _context.Tramites.Add(
                   new Tramite() {
                       Nombre = "Certificado de Obras Municipales",
                       Descripcion = "Las DOM son las encargadas de velar por el cumplimiento, a nivel comunal, de las normas que regulan la planificación urbana y la edificación, por ello, quienes quieran llevar a cabo un proyecto de edificación o urbanización deben obtener en estos servicios la autorización respectiva y los distintos certificados emitidos por la DOM.", 
                       SeccionId = 1,
                       Solicitudes = 0
                   }
                );
                _context.Tramites.Add(
                   new Tramite() {
                       Nombre = "Permiso de Edificación",
                       Descripcion= "El permiso de edificación o permiso de obra menor es una autorización para poder construir una obra. Se trata de la aprobación del conjunto de planos, informes técnicos, declaraciones y documentos que contienen el proyecto elaborado por el arquitecto que se contrató para tales fines.",
                       SeccionId = 1,
                       Solicitudes = 0
                   }
                );
                _context.Tramites.Add(
                   new Tramite() {
                       Nombre = " Rebaja en Tratamientos Medicos",
                       SeccionId = 2,
                       Solicitudes = 0
                   }
                );
                _context.Tramites.Add(
                  new Tramite() {
                      Nombre = "Rebaja en Mensualidad de Colegios Particulares",
                      SeccionId = 2,
                      Solicitudes = 0
                  }
               );

            }
            await _context.SaveChangesAsync();

            if(empleados.Count() == 0) { // se verifica si hay empleados
                //si no los hay se crean
                _context.Empleados.Add(
                    new Empleado() {
                        Run = "99.999.999-9",
                        Nombre = "usuario1 usuario1",
                        RolId = 4,
                        SeccionId = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
                _context.Empleados.Add(
                    new Empleado() {
                        Run = "88.888.888-8",
                        Nombre = "usuario2 usuario2",
                        RolId = 3,
                        SeccionId = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
                _context.Empleados.Add(
                    new Empleado() {
                        Run = "77.777.777-7",
                        Nombre = "usuario3 usuario3",
                        RolId = 2,
                        SeccionId = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
            }
            await _context.SaveChangesAsync();

            if(usuarios.Count() == 0) {// se verifica si hay usuarios
                //si no los hay se crean
                _context.Usuarios.Add(
                    new Usuario() {
                        Run = "99.999.999-9",
                        Nombre = "usuario1 usuario1",
                        Mail = "usuario1@usuario.cl",
                        Rolid = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
                _context.Usuarios.Add(
                    new Usuario() {
                        Run = "88.888.888-8",
                        Nombre = "usuario2 usuario2",
                        Mail = "usuario2@usuario.cl",
                        Rolid = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
                _context.Usuarios.Add(
                    new Usuario() {
                        Run = "77.777.777-7",
                        Nombre = "usuario3 usuario3",
                        Mail = "usuario3@usuario.cl",
                        Rolid = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
                _context.Usuarios.Add(
                    new Usuario() {
                        Run = "66.666.666-6",
                        Nombre = "usuario4 usuario4",
                        Mail = "usuario4@usuario.cl",
                        Rolid = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
                _context.Usuarios.Add(
                    new Usuario() {
                        Run = "55.555.555-5",
                        Nombre = "usuario5 usuario5",
                        Mail = "usuario5@usuario.cl",
                        Rolid = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
                _context.Usuarios.Add(
                        new Usuario() {
                            Run = "44.444.444-4",
                            Nombre = "usuario6 usuario6",
                            Mail = "usuario6@usuario.cl",
                            Rolid = 1,
                            PasswordHash = passwordHash,
                            PasswordSalt = passwordSalt
                        }
                    );
            }
            await _context.SaveChangesAsync();

            return View();
        }

        public async Task<IActionResult> LoginTrabajador() {
            var usuarios = _context.Usuarios.ToList(); // se obtienen todos los usuarios
            var empleados = _context.Empleados.ToList(); // se obtienen todos los empleados
            var roles = _context.Roles.ToList(); // se obtienen todos los roles 
            var secciones = _context.Secciones.ToList(); // se obtienen todos los roles 
            var tramites = _context.Tramites.ToList(); // se obtienen todos los roles 

            if(roles.Count() == 0) { // se verifica si hay roles
                //si no los hay se crean
                _context.Roles.Add(
                    new Rol() {
                        Nombre = "Usuario",
                        Descripcion = "El usuario solo tíene acceso a su propia lista de horas pedidas por él."
                    }
                );

                _context.Roles.Add(
                    new Rol() {
                        Nombre = "Empleado",
                        Descripcion = "El empleado tiene acceso a todas las horas pedidas en la sección que tiene asignada."
                    }
                );

                _context.Roles.Add(
                    new Rol() {
                        Nombre = "JefeSeccion",
                        Descripcion = "El jefe de sección tiene acceso a todo lo relacionado con su sección."
                    }
                );

                _context.Roles.Add(
                    new Rol() {
                        Nombre = "Admin",
                        Descripcion = "El administrador tiene acceso a todas las secciones."
                    }
                );
            }
            await _context.SaveChangesAsync();

            if(secciones.Count() == 0) { // se verifica si hay secciones
                //si no los hay se crean
                _context.Secciones.Add(
                    new Seccion() {
                        Nombre = "Dirección de Obras Municipales",
                        Descripcion = "Dirección de Obras Municipales"
                    }
                );
                _context.Secciones.Add(
                    new Seccion() {
                        Nombre = "Dirección de Desarrollo Comunitario",
                        Descripcion = "Dirección de Desarrollo Comunitario"
                    }
                );

            }
            await _context.SaveChangesAsync();

            if(tramites.Count() == 0) { // se verifica si hay secciones
                //si no los hay se crean
                _context.Tramites.Add(
                   new Tramite() {
                       Nombre = "Tramite 1",
                       SeccionId = 1,
                       Solicitudes = 0
                   }
                );
                _context.Tramites.Add(
                   new Tramite() {
                       Nombre = "Tramite 2",
                       SeccionId = 1,
                       Solicitudes = 0
                   }
                );
                _context.Tramites.Add(
                   new Tramite() {
                       Nombre = "Tramite 3",
                       SeccionId = 2,
                       Solicitudes = 0
                   }
                );
                _context.Tramites.Add(
                  new Tramite() {
                      Nombre = "Tramite 4",
                      SeccionId = 2,
                      Solicitudes = 0
                  }
               );

            }
            await _context.SaveChangesAsync();

            if(empleados.Count() == 0) { // se verifica si hay empleados
                //si no los hay se crean
                _context.Empleados.Add(
                    new Empleado() {
                        Run = "99.999.999-9",
                        Nombre = "usuario1 usuario1",
                        RolId = 4,
                        SeccionId = 1
                    }
                );
                _context.Empleados.Add(
                    new Empleado() {
                        Run = "88.888.888-8",
                        Nombre = "usuario2 usuario2",
                        RolId = 3,
                        SeccionId = 1
                    }
                );
                _context.Empleados.Add(
                    new Empleado() {
                        Run = "77.777.777-7",
                        Nombre = "usuario3 usuario3",
                        RolId = 2,
                        SeccionId = 1
                    }
                );
            }
            await _context.SaveChangesAsync();

            CratePassHash("123456", out byte[] passwordHash, out byte[] passwordSalt);
            if(usuarios.Count() == 0) {// se verifica si hay usuarios
                //si no los hay se crean
                _context.Usuarios.Add(
                    new Usuario() {
                        Run = "99.999.999-9",
                        Nombre = "usuario1 usuario1",
                        Mail = "usuario1@usuario.cl",
                        Rolid = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
                _context.Usuarios.Add(
                    new Usuario() {
                        Run = "88.888.888-8",
                        Nombre = "usuario2 usuario2",
                        Mail = "usuario2@usuario.cl",
                        Rolid = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
                _context.Usuarios.Add(
                    new Usuario() {
                        Run = "77.777.777-7",
                        Nombre = "usuario3 usuario3",
                        Mail = "usuario3@usuario.cl",
                        Rolid = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
                _context.Usuarios.Add(
                    new Usuario() {
                        Run = "66.666.666-6",
                        Nombre = "usuario4 usuario4",
                        Mail = "usuario4@usuario.cl",
                        Rolid = 1,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }
                );
            }
            await _context.SaveChangesAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginIn(LoginViewModel Lvm) {
            if(Lvm.Run != null && Lvm.Password != null) {

                var us = _context.Usuarios.Where(u => u.Run.Equals(Lvm.Run)).FirstOrDefault();
                if(us != null) {
                    // usuario encontrado
                    if(verificarPass(Lvm.Password, us.PasswordHash, us.PasswordSalt)) {
                        // coincide usuario y contraseña
                        var Claims = new List<Claim> { // se genera una lista de claim donde se almacenan los datos necesarios para su identificación
                        new Claim(ClaimTypes.Name,us.Nombre), // se le asigna el nombre al claim
                        new Claim(ClaimTypes.NameIdentifier,us.Run), // se asigna le asigna el run al claim
                        new Claim(ClaimTypes.Role,us.Rolid.ToString()) // se le asigna el rol al claim
                    };

                        var identity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        return RedirectToAction("MiUsuario", "Turnos");

                    } else {
                        // coincide usuario pero no contraseña
                        ModelState.AddModelError("", "RUN o Contraseña incorrecta");
                        return View(Lvm);
                    }

                } else {
                    // usuario no encontrado
                    ModelState.AddModelError("", "RUN o Contraseña incorrecta");
                    return View(Lvm);
                }
            } else {
                ModelState.AddModelError("", "RUN o Contraseña incorrecta");
                return View(Lvm);
            }

        }

        [HttpPost]
        public async Task<IActionResult> LoginTrabajador(LoginViewModel Lvm) {
            if(Lvm.Run != null && Lvm.Password != null) {

                var us = _context.Empleados.Where(u => u.Run.Equals(Lvm.Run)).FirstOrDefault();
                if(us != null) {
                    // usuario encontrado
                    if(verificarPass(Lvm.Password, us.PasswordHash, us.PasswordSalt)) {
                        // coincide usuario y contraseña
                        var Claims = new List<Claim> { // se genera una lista de claim donde se almacenan los datos necesarios para su identificación
                        new Claim(ClaimTypes.Name,us.Nombre), // se le asigna el nombre al claim
                        new Claim(ClaimTypes.NameIdentifier,us.Run), // se asigna le asigna el run al claim
                        new Claim(ClaimTypes.Role,us.RolId.ToString()), // se le asigna el rol al claim
                        new Claim("Seccion",us.SeccionId.ToString()) // se le asigna el rol al claim
                    };

                        var identity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        return RedirectToAction("dashboard", "Trabajador");

                    } else {
                        // coincide usuario pero no contraseña
                        ModelState.AddModelError("", "RUN o Contraseña incorrecta");
                        return View(Lvm);
                    }

                } else {
                    // usuario no encontrado
                    ModelState.AddModelError("", "RUN o Contraseña incorrecta");
                    return View(Lvm);
                }
            } else {
                ModelState.AddModelError("", "RUN o Contraseña incorrecta");
                return View(Lvm);
            }

        }

        public async Task<IActionResult> LogOut() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Inicio", "Home");
        }

        private void CratePassHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using(var hmac = new HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool verificarPass(string password, byte[] passwordHash, byte[] passwordSalt) {
            using(var hmac = new HMACSHA512(passwordSalt)) {
                var pass = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return pass.SequenceEqual(passwordHash);
            }
        }
    }
}
