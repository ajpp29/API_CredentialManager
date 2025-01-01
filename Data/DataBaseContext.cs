using API_CredentialManager.Models;
using Microsoft.EntityFrameworkCore;

namespace API_CredentialManager.Data
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

        // Agregar DbSet para cada tabla
        // public DbSet<Estudiante> Estudiantes { get; set; }

        public DbSet<Servidor> Servidores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
