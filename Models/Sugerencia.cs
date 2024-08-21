namespace Tesis.Models {
    public class Sugerencia {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string TipoSugerencia { get; set; }
        public int SeccionId { get; set; } // variable para la id de la sección
        public Seccion Seccion { get; set; }
        public string Texto { get; set; }

    }
}
