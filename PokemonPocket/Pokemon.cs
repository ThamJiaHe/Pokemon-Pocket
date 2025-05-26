using System;
using System.Collections.Generic;
// Author: THAM JIA HE (244548T)
// Date: 2025-05-10
// Copyright (c) 2025 THAM JIA HE. All rights reserved.


namespace PokemonPocket
{
    public class PokemonMaster
    {
        public string Name { get; set; }
        public int NoToEvolve { get; set; }
        public string EvolveTo { get; set; }

        public PokemonMaster(string name, int noToEvolve, string evolveTo)
        {
            this.Name = name;
            this.NoToEvolve = noToEvolve;
            this.EvolveTo = evolveTo;
        }
    }

    // Base Pokemon class
    public abstract class Pokemon
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int Exp { get; set; }
        public string Skill { get; protected set; }
        public int SkillDamage { get; protected set; }

        public Pokemon(string name, int hp, int exp)
        {
            Name = name;
            HP = hp;
            Exp = exp;
        }

        // Abstract method to calculate damage based on the Pokémon type
        public abstract void CalculateDamage(int strikerDamage);

        // Override ToString to display Pokemon information
        public override string ToString()
        {
            return $"Name: {Name}, HP: {HP}, Exp: {Exp}, Skill: {Skill}, Skill Damage: {SkillDamage}";
        }
    }

    // Pikachu subclass
    public class Pikachu : Pokemon
    {
        public Pikachu(string name, int hp, int exp) : base(name, hp, exp)
        {
            Skill = "Lightning Bolt";
            SkillDamage = 30;
        }

        public override void CalculateDamage(int strikerDamage)
        {
            int damage = 3 * strikerDamage;
            HP -= damage;
            if (HP < 0) HP = 0;
        }
    }

    // Eevee subclass
    public class Eevee : Pokemon
    {
        public Eevee(string name, int hp, int exp) : base(name, hp, exp)
        {
            Skill = "Run Away";
            SkillDamage = 25;
        }

        public override void CalculateDamage(int strikerDamage)
        {
            int damage = 2 * strikerDamage;
            HP -= damage;
            if (HP < 0) HP = 0;
        }
    }

    // Charmander subclass
    public class Charmander : Pokemon
    {
        public Charmander(string name, int hp, int exp) : base(name, hp, exp)
        {
            Skill = "Solar Power";
            SkillDamage = 10;
        }

        public override void CalculateDamage(int strikerDamage)
        {
            int damage = 1 * strikerDamage;
            HP -= damage;
            if (HP < 0) HP = 0;
        }
    }

    // Evolved Pokemon classes
    public class Raichu : Pokemon
    {
        public Raichu(string name, int hp, int exp) : base(name, hp, exp)
        {
            Skill = "Lightning Bolt";  // Same skill as Pikachu
            SkillDamage = 30;          // Same skill damage as Pikachu
        }

        public override void CalculateDamage(int strikerDamage)
        {
            int damage = 3 * strikerDamage;
            HP -= damage;
            if (HP < 0) HP = 0;
        }
    }

    public class Flareon : Pokemon
    {
        public Flareon(string name, int hp, int exp) : base(name, hp, exp)
        {
            Skill = "Run Away";  // Same skill as Eevee
            SkillDamage = 25;    // Same skill damage as Eevee
        }

        public override void CalculateDamage(int strikerDamage)
        {
            int damage = 2 * strikerDamage;
            HP -= damage;
            if (HP < 0) HP = 0;
        }
    }

    public class Charmeleon : Pokemon
    {
        public Charmeleon(string name, int hp, int exp) : base(name, hp, exp)
        {
            Skill = "Solar Power";  // Same skill as Charmander
            SkillDamage = 10;       // Same skill damage as Charmander
        }

        public override void CalculateDamage(int strikerDamage)
        {
            int damage = 1 * strikerDamage;
            HP -= damage;
            if (HP < 0) HP = 0;
        }
    }
}