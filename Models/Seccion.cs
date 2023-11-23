namespace Tesis.Models {
    public class Seccion {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<Turno> Turnos { get; set; }
        public List<Empleado> Empleados { get; set; }
    }
}
