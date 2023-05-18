using System.ComponentModel.DataAnnotations;

namespace Tesis.Models {
    public class Seccion {
        [Key]
        public int? Id {get; set;}
        [Required]
        public string? Nombre { get; set;}
        [Required]
        public string? Descripcion { get; set;}
    }
}
