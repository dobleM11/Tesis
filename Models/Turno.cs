namespace Tesis.Models {
    public class Turno {
        public int Id { get; set; } // numero de identificación del turno
        public string UsuarioRun { get; set; } // variable del run del usuario
        public Usuario Usuario { get; set; }
        //public DateTime Fecha { get; set; } // variable de la feccha
        //public TimeSpan Hora { get; set; } // variable de la hora
        public DateTime FechaHora { get; set; } // variable de la fecha y hora

        public int SeccionId { get; set; } // variable de la id de la sección
        public Seccion Seccion { get; set; }
        public bool Asistencia { get; set; } // se marca si se asistió o no a la cita

    }
}
