using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace Tesis.Models {
    public class EnviarAviso : IHostedService {
        private Timer _timer;
        private readonly AppDbContext _appDbContext;

        public EnviarAviso(AppDbContext appDbContext) {
            _appDbContext = appDbContext;
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            // Calcula el tiempo hasta las 8 de la mañana
            DateTime now = DateTime.Now;
            DateTime scheduledTime = DateTime.Today.AddHours(8);

            if(now > scheduledTime) {
                scheduledTime = scheduledTime.AddDays(1);
            }

            TimeSpan timeUntil8AM = scheduledTime - now;

            // Configura el temporizador para que se ejecute a las 8 de la mañana
            _timer = new Timer(EnviarCorreo, null, timeUntil8AM, TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private void EnviarCorreo(object? state) {
            var turnosDiaSiguiente = _appDbContext.Turnos.Include(a => a.Usuario).Include(a => a.Seccion).Where(a => a.FechaHora.Date == DateTime.Now.AddDays(1).Date);
            foreach(var item in turnosDiaSiguiente) {
                string remitente = "aviso@municipalidad.com";
                string destinatario = item.Usuario.Mail;
                string asunto = "Recordatorio de turno";
                string mensaje = "El día " + DateTime.Now.AddDays(1).Date + "usted tiene una hora registrada para " + item.Seccion.Nombre + "a las " + item.FechaHora.TimeOfDay + ".";
                SmtpClient smtpClient = new SmtpClient("servidor_smtp");
                smtpClient.Credentials = new NetworkCredential("usuario", "contraseña");

                MailMessage mailMessage = new MailMessage(remitente, destinatario, asunto, mensaje);

                smtpClient.Send(mailMessage);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken) {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
