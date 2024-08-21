namespace Tesis.Models {
    public class Tramite {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Solicitudes { get; set; }

        public int SeccionId { get; set; } // Esto es la clave externa
        public Seccion Seccion { get; set; } // Esto es la propiedad de navegación


    }
}
