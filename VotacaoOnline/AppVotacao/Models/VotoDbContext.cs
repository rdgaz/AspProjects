using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AppVotacao.Models
{
    public partial class VotoDbContext : DbContext
    {

        public DbSet<Voto> Votos { get; set; }
        //public DbSet<Usuario> Usuarios { get; set; }
        //public DbSet<Restaurante> Restaurantes { get; set; }

        public VotoDbContext()
        {
        }

        public VotoDbContext(DbContextOptions<VotoDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlite("Data Source=local.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
