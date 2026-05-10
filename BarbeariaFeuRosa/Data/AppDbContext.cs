using BarbeariaFeuRosa.Models;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaFeuRosa.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Barbearia> Barbearias { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Servico> Servicos { get; set; }

        public DbSet<Configuracao> Configuracoes { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Barbeiro> Barbeiros { get; set; }

        public DbSet<Agendamento> Agendamentos { get; set; }
    }
}