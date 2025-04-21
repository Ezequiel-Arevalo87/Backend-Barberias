using CrudApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario>? Usuarios { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<Barberia>? Barberias { get; set; }
        public DbSet<Barbero>? Barberos { get; set; }
        public DbSet<Service> Servicios { get; set; }
        public DbSet<TipoDocumento> TipoDocumento { get; set; }
        public DbSet<SucursalBarberia> SucursalesBarberia { get; set; }

        public DbSet<HorarioBarberia> HorariosBarberia { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Turno> Turnos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Usuario - Role
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario - Cliente (1:1)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Cliente)
                .WithOne(c => c.Usuario)
                .HasForeignKey<Cliente>(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario - Barbero (1:1)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Barbero)
                .WithOne(b => b.Usuario)
                .HasForeignKey<Barbero>(b => b.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario - Barberia (1:1)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Barberia)
                .WithOne(b => b.Usuario)
                .HasForeignKey<Barberia>(b => b.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Barbería - Sucursales
            modelBuilder.Entity<SucursalBarberia>()
                .HasOne(sb => sb.Barberia)
                .WithMany(b => b.Sucursales)
                .HasForeignKey(sb => sb.BarberiaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Barbero>()
               .HasOne(b => b.Sucursal)
               .WithMany()
               .HasForeignKey(b => b.SucursalId)
               .OnDelete(DeleteBehavior.Restrict);

            // Barbería - Barberos
            modelBuilder.Entity<Barbero>()
                .HasOne(b => b.Barberia)
                .WithMany(barb => barb.Barberos)
                .HasForeignKey(b => b.BarberiaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Barbería - Horarios
            modelBuilder.Entity<HorarioBarberia>()
                .HasOne(h => h.Barberia)
                .WithMany(b => b.Horarios)
                .HasForeignKey(h => h.BarberiaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Turno - Barbero
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Barbero)
                .WithMany(b => b.Turnos)
                .HasForeignKey(t => t.BarberoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Turno - Cliente
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Cliente)
                .WithMany(c => c.Turnos)
                .HasForeignKey(t => t.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Turno - Servicio
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Servicio)
                .WithMany(s => s.Turnos)
                .HasForeignKey(t => t.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices Únicos
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Nombre)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
