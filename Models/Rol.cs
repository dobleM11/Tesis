namespace Tesis.Models {
    public class Rol {
        public int Id { get; set; } // id del rol, se genera automáticamente
        public string? Nombre { get; set; } //nombre del rol
        public string? Descripcion { get; set; } // breve descripción del rol
        public List<Empleado> Empleados { get; set; }
        public List<Usuario> Usuarios { get; set; }
    }
}
