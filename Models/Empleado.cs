using System.ComponentModel.DataAnnotations;

namespace Tesis.Models {
    public class Empleado {
        [Key]
        public string? Rut { get; set; }
        [Required]
        public string? Nombre { get; set; }
        [Required]
        public int IdRol { get; set; }
        [Required]
        public int IdSeccion { get; set; }

    }
}
