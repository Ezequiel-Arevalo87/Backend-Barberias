using CrudApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario>? Usuarios { get; set; }
        public DbSet<Role>? Roles { get; set; }  // Agregado

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación entre Usuario y Role
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Role)
                .WithMany() // Si Role tiene una lista de Usuarios, usa .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RoleId);

            // Opcional: Si necesitas un índice único en los roles
            modelBuilder.Entity<Role>().HasIndex(r => r.Nombre).IsUnique();
        }
    }
}
