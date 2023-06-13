using Microsoft.EntityFrameworkCore;

namespace Tesis.Models {
    public class AppDbContext : DbContext {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Sugerencia> Sugerencias { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=DbTesis;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().HasKey(x => x.Run); //aquí se setea la clave primaria
            modelBuilder.Entity<Empleado>().HasKey(x => x.Run); //aquí se setea la clave primaria
            modelBuilder.Entity<Rol>().HasKey(x => x.Id); //aquí se setea la clave primaria
            modelBuilder.Entity<Seccion>().HasKey(x => x.Id); //aquí se setea la clave primaria
            modelBuilder.Entity<Turno>().HasKey(x => x.Id); //aquí se setea la clave primaria
            modelBuilder.Entity<Sugerencia>().HasKey(x => x.Id); //aquí se setea la clave primaria
        }
    }
}
