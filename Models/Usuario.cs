namespace Tesis.Models {
    public class Usuario {
        public string Run { get; set; } // run del usuario
        public string Nombre { get; set; } // nombre del usuario ojalá conmbre completo (primer nombre, segundo nombre, primer apellido, segundo apellido)
        public string Mail { get; set; }
        public int Rolid { get; set; } // variable para el id del rol
        public Rol Rol { get; set; }
        public byte[] PasswordHash { get; set; } // hash para tener una contraseña segura
        public byte[] PasswordSalt { get; set; } // salt que es la key de la contraseña
        public List<Turno> Turnos { get; set; }
    }
}
