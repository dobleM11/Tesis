using System.ComponentModel.DataAnnotations;

namespace Tesis.Models {
    public class Turno {
        [Key]
        public int Id { get; set; }
        [Required]
        public string NombreCliente { get; set; }
        [Required]
        public string RutCliente { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public TimeSpan Hora { get; set; }
        [Required]
        public int SeccionId { get; set; }
        [Required]
        public string EmpleadoRut { get; set; }

    }
}
