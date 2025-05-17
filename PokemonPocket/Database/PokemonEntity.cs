using System;

namespace PokemonPocket.Database
{
    // Entity class for storing Pokemon data in the database
    public class PokemonEntity
    {
        // Primary key for the database table
        public int Id { get; set; }
        
        // The name given to the Pokemon
        public string Name { get; set; }
        
        // The type of Pokemon (Pikachu, Eevee, etc.)
        public string Type { get; set; }
        
        // Current HP of the Pokemon
        public int HP { get; set; }
        
        // Current Experience points
        public int Exp { get; set; }
        
        // The Pokemon's skill name
        public string Skill { get; set; }
        
        // The damage value of the Pokemon's skill
        public int SkillDamage { get; set; }
    }

    // Entity class for storing Pokemon evolution data in the database
    public class PokemonMasterEntity
    {
        // Primary key for the database table
        public int Id { get; set; }
        
        // The type of Pokemon that can evolve
        public string Name { get; set; }
        
        // How many Pokemon are needed to evolve
        public int NoToEvolve { get; set; }
        
        // What the Pokemon evolves into
        public string EvolveTo { get; set; }
    }
}