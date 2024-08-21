namespace Tesis.Models {
    public class Turno {
        public int Id { get; set; } // Número de identificación del turno
        public string UsuarioRun { get; set; } // Variable del RUN del usuario
        public Usuario Usuario { get; set; }

        public DateTime Fecha { get; set; } // Variable de la fecha
        public TimeSpan Hora { get; set; } // Variable de la hora
        public TimeSpan HoradeEntrada { get; set; } // Variable de la hora

        public int TramiteId { get; set; } // Variable de la ID de la sección
        public Tramite Tramite { get; set; }

        public int Asistencia { get; set; } // Se marca si se asistió o no a la cita 0=null, 1=no, 2=si
    }
}
