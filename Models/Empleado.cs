//esta clase es para tener un registro de quien atendió el tramite

namespace Tesis.Models {
    public class Empleado {
        public string Run { get; set; } // run del empleado
        public string Nombre { get; set; } // nombre del empleado ojalá conmbre completo (primer nombre, segundo nombre, primer apellido, segundo apellido)
        //public byte[] PasswordHash { get; set; } // hash para tener una contraseña segura
        //public byte[] PasswordSalt { get; set; } // salt que es la key de la contraseña

        public int RolId { get; set; } // variable para la id del rol
        public Rol Rol { get; set; }
        public int SeccionId { get; set; } // variable para la id de la sección
        public Seccion Seccion { get; set; }
        public byte[] PasswordHash { get; set; } // hash para tener una contraseña segura
        public byte[] PasswordSalt { get; set; } // salt que es la key de la contraseña
    }
}
