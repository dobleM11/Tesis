using System.ComponentModel.DataAnnotations;

namespace Tesis.Models {
    public class Rol {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }

    }
}
