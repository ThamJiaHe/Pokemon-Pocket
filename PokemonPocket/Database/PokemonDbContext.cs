using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace PokemonPocket.Database
{
    public class PokemonDbContext : IDisposable
    {
        private readonly SqliteConnection _connection; // SQLite connection to the database
        private bool _disposed = false; 
        
        // Collection to hold Pokemon entities loaded from the database
        public List<PokemonEntity> Pokemons { get; private set; }
        
        // Collection to hold PokemonMaster entities loaded from the database
        public List<PokemonMasterEntity> PokemonMasters { get; private set; }
        // Initializes the database context
        public PokemonDbContext(string databasePath = null)
        {
            // Use provided path or default path
            if (string.IsNullOrEmpty(databasePath))
            {
                string databaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
                databasePath = Path.Combine(databaseDirectory, "pokemon_pocket.db");
                
                // Ensure the directory exists
                Directory.CreateDirectory(databaseDirectory);
            }
            
            // Create connection string and open connection
            string connectionString = $"Data Source={databasePath}";
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
            
            // Initialize collections
            Pokemons = new List<PokemonEntity>();
            PokemonMasters = new List<PokemonMasterEntity>();
            
            // Ensure database tables exist
            EnsureTablesExist();
            
            // Load data from the database
            LoadPokemonsFromDatabase();
            LoadPokemonMastersFromDatabase();
        }

        // Create database tables if they don't exist
        private void EnsureTablesExist()
        {
            using (var command = _connection.CreateCommand())
            {
                // Create Pokemons table
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Pokemons (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Type TEXT NOT NULL,
                        HP INTEGER NOT NULL,
                        Exp INTEGER NOT NULL,
                        Skill TEXT NOT NULL,
                        SkillDamage INTEGER NOT NULL
                    )";
                command.ExecuteNonQuery();
            }
            
            using (var command = _connection.CreateCommand())
            {
                // Create PokemonMasters table
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS PokemonMasters (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        NoToEvolve INTEGER NOT NULL,
                        EvolveTo TEXT NOT NULL
                    )";
                command.ExecuteNonQuery();
            }
        }

        // Check if database has been initialized with seed data
        public bool EnsureCreated()
        {
            // Check if PokemonMasters table has any data
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM PokemonMasters";
                long count = Convert.ToInt64(command.ExecuteScalar());
                return count > 0;
            }
        }

        // Load Pokemon entities from the database
        private void LoadPokemonsFromDatabase()
        {
            Pokemons.Clear();
            
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name, Type, HP, Exp, Skill, SkillDamage FROM Pokemons";
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pokemon = new PokemonEntity
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Type = reader.GetString(2),
                            HP = reader.GetInt32(3),
                            Exp = reader.GetInt32(4),
                            Skill = reader.GetString(5),
                            SkillDamage = reader.GetInt32(6)
                        };
                        
                        Pokemons.Add(pokemon);
                    }
                }
            }
        }

        // Load PokemonMaster entities from the database
        private void LoadPokemonMastersFromDatabase()
        {
            PokemonMasters.Clear();
            
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name, NoToEvolve, EvolveTo FROM PokemonMasters";
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var master = new PokemonMasterEntity
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            NoToEvolve = reader.GetInt32(2),
                            EvolveTo = reader.GetString(3)
                        };
                        
                        PokemonMasters.Add(master);
                    }
                }
            }
        }

        // Save changes to the database
        public void SaveChanges()
        {
            // First, remove all Pokemon from the database
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Pokemons";
                command.ExecuteNonQuery();
            }
            
            // Then insert all Pokemon in our collection
            if (Pokemons.Count > 0)
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    using (var command = _connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT INTO Pokemons (Name, Type, HP, Exp, Skill, SkillDamage) 
                            VALUES (@Name, @Type, @HP, @Exp, @Skill, @SkillDamage)";
                        
                        // Create parameters
                        var nameParam = command.CreateParameter();
                        nameParam.ParameterName = "@Name";
                        command.Parameters.Add(nameParam);
                        
                        var typeParam = command.CreateParameter();
                        typeParam.ParameterName = "@Type";
                        command.Parameters.Add(typeParam);
                        
                        var hpParam = command.CreateParameter();
                        hpParam.ParameterName = "@HP";
                        command.Parameters.Add(hpParam);
                        
                        var expParam = command.CreateParameter();
                        expParam.ParameterName = "@Exp";
                        command.Parameters.Add(expParam);
                        
                        var skillParam = command.CreateParameter();
                        skillParam.ParameterName = "@Skill";
                        command.Parameters.Add(skillParam);
                        
                        var skillDamageParam = command.CreateParameter();
                        skillDamageParam.ParameterName = "@SkillDamage";
                        command.Parameters.Add(skillDamageParam);
                        
                        // Insert each Pokemon
                        foreach (var pokemon in Pokemons)
                        {
                            nameParam.Value = pokemon.Name;
                            typeParam.Value = pokemon.Type;
                            hpParam.Value = pokemon.HP;
                            expParam.Value = pokemon.Exp;
                            skillParam.Value = pokemon.Skill;
                            skillDamageParam.Value = pokemon.SkillDamage;
                            
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        // Seed the PokemonMasters table with initial evolution data
        public void SeedPokemonMasters(List<PokemonMaster> masters)
        {
            // First clear existing data
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM PokemonMasters";
                command.ExecuteNonQuery();
            }
            
            // Then insert new data
            using (var transaction = _connection.BeginTransaction())
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO PokemonMasters (Name, NoToEvolve, EvolveTo)
                        VALUES (@Name, @NoToEvolve, @EvolveTo)";
                    
                    // Create parameters
                    var nameParam = command.CreateParameter();
                    nameParam.ParameterName = "@Name";
                    command.Parameters.Add(nameParam);
                    
                    var noToEvolveParam = command.CreateParameter();
                    noToEvolveParam.ParameterName = "@NoToEvolve";
                    command.Parameters.Add(noToEvolveParam);
                    
                    var evolveToParam = command.CreateParameter();
                    evolveToParam.ParameterName = "@EvolveTo";
                    command.Parameters.Add(evolveToParam);
                    
                    // Insert each master
                    foreach (var master in masters)
                    {
                        nameParam.Value = master.Name;
                        noToEvolveParam.Value = master.NoToEvolve;
                        evolveToParam.Value = master.EvolveTo;
                        
                        command.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            
            // Reload PokemonMasters from database
            LoadPokemonMastersFromDatabase();
        }

        // Add a new Pokemon entity to the collection
        public void Add(PokemonEntity entity)
        {
            Pokemons.Add(entity);
        }

        // Remove all Pokemon entities from the collection
        public void RemoveRange(IEnumerable<PokemonEntity> entities)
        {
            foreach (var entity in entities)
            {
                Pokemons.Remove(entity);
            }
        }

        // Dispose method to properly close the database connection
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Helper method for implementing IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
                _disposed = true;
            }
        }

        // Finalizer
        ~PokemonDbContext()
        {
            Dispose(false);
        }
    }
}