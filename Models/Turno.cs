namespace Tesis.Models {
    public class Turno {
        public int Id { get; set; } // numero de identificación del turno
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public string UsuarioRun { get; set; } // variable del run del usuario
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public Usuario Usuario { get; set; }
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        //public DateTime Fecha { get; set; } // variable de la feccha
        //public TimeSpan Hora { get; set; } // variable de la hora
        public DateTime FechaHora { get; set; } // variable de la fecha y hora

        public int SeccionId { get; set; } // variable de la id de la sección
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public Seccion Seccion { get; set; }
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public bool Asistencia { get; set; } // se marca si se asistió o no a la cita

    }
}
