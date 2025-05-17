using System;
using System.IO;

namespace PokemonPocket.Database
{
    public class PokemonDbContext : DbContext
    {
        // Path to the database file in the Database folder
        private static readonly string DatabasePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Database", "pokemon_pocket.db");

        // Define the database tables
        public DbSet<PokemonEntity> Pokemons { get; set; }
        public DbSet<PokemonMasterEntity> PokemonMasters { get; set; }

        // Configure the database connection
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Create the Database directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath));
            
            // Configure SQLite with the database file path
            optionsBuilder.UseSqlite($"Data Source={DatabasePath}");
        }

        // Configure the database model and seed initial data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed initial PokemonMaster data
            modelBuilder.Entity<PokemonMasterEntity>().HasData(
                new PokemonMasterEntity { Id = 1, Name = "Pikachu", NoToEvolve = 2, EvolveTo = "Raichu" },
                new PokemonMasterEntity { Id = 2, Name = "Eevee", NoToEvolve = 3, EvolveTo = "Flareon" },
                new PokemonMasterEntity { Id = 3, Name = "Charmander", NoToEvolve = 1, EvolveTo = "Charmeleon" }
            );
        }
    }
}