using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.IO;
using System.Data;
using Microsoft.Data.Sqlite;
using PokemonPocket.Database;


namespace PokemonPocket
{
    class Program
    {
        private static readonly string SaveFilePath = "pokemon_save.csv";
        private static readonly string DatabaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
        private static readonly string DatabasePath = Path.Combine(DatabaseDirectory, "pokemon_pocket.db");

        static void Main(string[] args)
        {
            // This list holds the evolution information for Pokémon
            List<PokemonMaster> pokemonMasters = new List<PokemonMaster>(){
                new PokemonMaster("Pikachu", 2, "Raichu"),
                new PokemonMaster("Eevee", 3, "Flareon"),
                new PokemonMaster("Charmander", 1, "Charmeleon")
            };
            
            // Initialize the database with evolution data
            InitializeDatabase(pokemonMasters);

            // Create a list to hold Pokemon objects
            List<Pokemon> pokemonPocket = new List<Pokemon>();
            
            // Try to load Pokemon from database when starting
            LoadPokemonFromDatabase(pokemonPocket);
            
            // Variable to hold the user's choice
            char choice;
            // Create a boolean variable to control the loop
            bool running = true;
            
            // Main loop to display the menu and handle user input
            while (running)
            {
                // Display the menu
                Console.WriteLine("*****************************");
                Console.WriteLine("Welcome to Pokémon Pocket! APP");
                Console.WriteLine("*****************************");
                Console.WriteLine("(1). Add pokemon to my pocket");
                Console.WriteLine("(2). List pokemon(s) in my Pocket");
                Console.WriteLine("(3). Check if I can evolve pokemon");
                Console.WriteLine("(4). Evolve pokemon");
                Console.WriteLine("(5). Battle Arena");
                Console.WriteLine("(6). Training Center");
                Console.WriteLine("(7). Save & Load");
                Console.WriteLine("(8). Load Pokemon from Database");
                Console.WriteLine("(9). Save Pokemon to Database");
                Console.Write("Please only enter [1,2,3,4,5,6,7,8,9] or Q to quit: ");

                // Read user input with validation
                try
                {
                    // Read the user's choice
                    choice = Console.ReadKey(true).KeyChar;
                    Console.WriteLine(choice);

                    // Process the user's choice
                    // Use a switch statement to handle different choices
                    switch (choice)
                    {
                        // Option 1: Add a new Pokémon to the pocket
                        case '1':
                            AddPokemon(pokemonPocket);
                            break;
                        // Option 2: List all Pokémon in the pocket
                        case '2':
                            ListPokemon(pokemonPocket);
                            break;
                        // Option 3: Check if I can evolve Pokémon
                        case '3':
                            CheckEvolution(pokemonPocket, pokemonMasters);
                            break;
                        // Option 4: Evolve Pokémon if possible
                        case '4':
                            EvolvePokemon(pokemonPocket, pokemonMasters);
                            break;
                        // Option 5: Battle Arena
                        case '5':
                            BattleArena();
                            break;
                        // Option 6: Training Center
                        case '6':
                            TrainPokemon(pokemonPocket);
                            break;
                        // Option 7: Save & Load
                        case '7':
                            SaveLoadMenu(pokemonPocket);
                            break;
                        // Option 8: Load Pokemon from Database
                        case '8':
                            LoadPokemonFromDatabase(pokemonPocket);
                            break;
                        // Option 9: Save Pokemon to Database
                        case '9':
                            SavePokemonToDatabase(pokemonPocket);
                            break;
                        // Exit the program if the user enters 'q'Lowercase or 'Q'Uppercase
                        case 'q':
                        case 'Q':
                            Console.WriteLine("Saving your Pokémon data before exiting...");
                            SavePokemonToDatabase(pokemonPocket);
                            Console.WriteLine("Thank you for using Pokémon Pocket! Goodbye!");
                            running = false;
                            break;
                        default:
                            // Handle invalid menu selection
                            Console.WriteLine("Invalid choice. Please enter a number between 1 and 9 or Q to quit.");
                            break;
                    }
                }

                catch (Exception ex)
                {
                    // Handle any exceptions that occur during input or processing
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        // Add new Pokémon to the pocket
        private static void AddPokemon(List<Pokemon> pokemonPocket)
        {
            try
            {
                // Prompt the user for Pokémon name 
                Console.WriteLine("Enter the name of the Pokémon: ");
                // Read the Pokémon name from the user
                string name = Console.ReadLine();

                // Check if the name is empty or null
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Name cannot be empty.");
                    return;
                }
                // Trim the input to remove leading and trailing whitespaces from the name
                name = name.Trim();

                // Determine the Pokémon type
                string pokemonType = DeterminePokemonType(name);

                // Validate the name corresponds to a valid Pokémon type
                if (pokemonType == null)
                {
                    Console.WriteLine("Invalid Pokémon type. Please enter a valid type.");
                    return;
                }

                // Prompt the user for Pokémon HP
                Console.WriteLine("Enter Pokémon's HP: ");

                // Validate the input to ensure it's a number
                if (!int.TryParse(Console.ReadLine(), out int hp))
                {
                    Console.WriteLine("Invalid HP. Please enter a valid number.");
                    return;
                }

                // Prompt the user for Pokémon EXP
                Console.WriteLine("Enter Pokémon's EXP: ");
                if (!int.TryParse(Console.ReadLine(), out int exp) || exp <= 0)
                {
                    Console.WriteLine("Invalid EXP. Please enter a positive number for EXP.");
                    return;
                }

                Pokemon newPokemon = null;

                // Create a new Pokémon object based on the type
                switch (pokemonType.ToLower())
                {
                    case "pikachu":
                        newPokemon = new Pikachu(name, hp, exp);
                        break;
                    case "eevee":
                        newPokemon = new Eevee(name, hp, exp);
                        break;
                    case "charmander":
                        newPokemon = new Charmander(name, hp, exp);
                        break;
                }

                pokemonPocket.Add(newPokemon);
                Console.WriteLine($"{name} was successfully added to your Pokémon Pocket!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static string DeterminePokemonType(string name)
        {
            string lowerName = name.ToLower();

            if (lowerName.Contains("pikachu"))
            {
                return "Pikachu";
            }
            else if (lowerName.Contains("eevee"))
            {
                return "Eevee";
            }
            else if (lowerName.Contains("charmander"))
            {
                return "Charmander";
            }
            else if (lowerName.Contains("raichu"))
            {
                return "Raichu";
            }
            else if (lowerName.Contains("flareon"))
            {
                return "Flareon";
            }
            else if (lowerName.Contains("charmeleon"))
            {
                return "Charmeleon";
            }
            else
            {
                return null; // Invalid Pokémon type
            }
        }

        private static void ListPokemon(List<Pokemon> pokemonPocket)
        {
            if (pokemonPocket.Count == 0)
            {
                Console.WriteLine("No Pokémon in your pocket.");
                return;
            }

            // Sort Pokemon by Experience points (Exp) in descending order
            var sortedPokemon = pokemonPocket.OrderByDescending(p => p.Exp).ToList();

            foreach (var pokemon in sortedPokemon)
            {
                Console.WriteLine($"Name: {pokemon.Name}");
                Console.WriteLine($"HP: {pokemon.HP}");
                Console.WriteLine($"Exp: {pokemon.Exp}");
                Console.WriteLine($"Skill: {pokemon.Skill}");
                Console.WriteLine("----------------------");
            }
        }

        private static void CheckEvolution(List<Pokemon> pokemonPocket, List<PokemonMaster> pokemonMasters)
        {
            if (pokemonPocket.Count == 0)
            {
                Console.WriteLine("No Pokémon in your pocket.");
                return;
            }

            bool canEvolve = false;
            foreach (PokemonMaster master in pokemonMasters)
            {
                int count = pokemonPocket.Count(p => p.GetType().Name == master.Name);

                if (count >= master.NoToEvolve)
                {
                    Console.WriteLine($"{count} {master.Name} --> {master.NoToEvolve} {master.EvolveTo}");
                    canEvolve = true;
                }
            }
            if (!canEvolve)
            {
                Console.WriteLine("You cannot evolve any Pokémon as you do not have enough EXP.");
            }
        }

        private static void EvolvePokemon(List<Pokemon> pokemonPocket, List<PokemonMaster> pokemonMasters)
        {
            if (pokemonPocket.Count == 0)
            {
                Console.WriteLine("No Pokemon in your pocket to evolve.");
                return;
            }

            bool anyEvolved = false;

            foreach (PokemonMaster master in pokemonMasters)
            {
                // Find all Pokemon of this type
                List<Pokemon> eligiblePokemon = pokemonPocket.Where(p => p.GetType().Name == master.Name).ToList();
                int count = eligiblePokemon.Count;

                if (count >= master.NoToEvolve)
                {
                    // Sort by lowest exp first to evolve those
                    eligiblePokemon = eligiblePokemon.OrderBy(p => p.Exp).ToList();

                    // Remove the Pokemon that will be evolved
                    for (int i = 0; i < master.NoToEvolve; i++)
                    {
                        Pokemon toRemove = eligiblePokemon[i];
                        pokemonPocket.Remove(toRemove);
                    }

                    // Create the evolved Pokemon
                    Pokemon evolvedPokemon = null;

                    switch (master.EvolveTo)
                    {
                        case "Raichu":
                            evolvedPokemon = new Raichu(master.EvolveTo, 100, 0);
                            break;
                        case "Flareon":
                            evolvedPokemon = new Flareon(master.EvolveTo, 100, 0);
                            break;
                        case "Charmeleon":
                            evolvedPokemon = new Charmeleon(master.EvolveTo, 100, 0);
                            break;
                    }
                    // Add the evolved Pokemon to the pocket
                    pokemonPocket.Add(evolvedPokemon);
                    Console.WriteLine($"{master.Name} evolved into {master.EvolveTo}!");
                    anyEvolved = true;
                }
            }

            // If no Pokemon were evolved, inform the user
            if (!anyEvolved)
            {
                Console.WriteLine("You don't have enough Pokemon to evolve.");
            }
        }

        private static void BattleArena()
        {
            Console.WriteLine("Welcome to the Battle Arena!");

            if (pokemonPocket.Count < 2)
            {
                Console.WriteLine("You need at least 2 Pokémon to battle.");
                return;
            }

            Console.WriteLine("Choose your Pokémon to battle: ");
            for (int i = 0; i < pokemonPocket.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {pokemonPocket[i].Name} {pokemonPocket[i].GetType().Name}, HP: {pokemonPocket[i].HP}, EXP: {pokemonPocket[i].Exp}");
            }

            Console.Write("\nSelect your first Pokémon (number): ");

            if (!int.TryParse(Console.ReadLine(), out int choice1) || choice1 < 1 || choice1 > pokemonPocket.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            // Select second Pokemon
            Console.Write("Select second Pokemon (number): ");
            if (!int.TryParse(Console.ReadLine(), out int choice2) ||
                choice2 < 1 || choice2 > pokemonPocket.Count ||
                choice1 == choice2)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            // Get the selected Pokémon
            Pokemon pokemon1 = pokemonPocket[choice1 - 1];
            Pokemon pokemon2 = pokemonPocket[choice2 - 1];

            // Store original HP values
            int originalHP1 = pokemon1.HP;
            int originalHP2 = pokemon2.HP;

            // Battle animation and setup
            Console.Clear();
            Console.WriteLine($"\n === Battle Start! {pokemon1.Name} VS {pokemon2.Name} ===");

            Console.WriteLine($"{pokemon1.Name} HP: {pokemon1.HP}");
            Console.WriteLine($"{pokemon2.Name} HP: {pokemon2.HP}");

            Console.WriteLine("3...");
            Thread.Sleep(800);
            Console.WriteLine("2...");
            Thread.Sleep(800);
            Console.WriteLine("1...");
            Thread.Sleep(800);
            Console.WriteLine("Battle Start!\n");

            // Battle rounds (maximum 3 rounds)
            for (int round = 1; round <= 3; round++)
            {
                Console.WriteLine($"--- Round {round} ---");

                // Pokemon 1 attacks
                Console.WriteLine($"{pokemon1.Name} uses {pokemon1.Skill} for {pokemon1.SkillDamage} damage!");
                int originalHP2BeforeAttack = pokemon2.HP;
                pokemon2.CalculateDamage(pokemon1.SkillDamage);
                Console.WriteLine($"{pokemon2.Name} HP decreased by {originalHP2BeforeAttack - pokemon2.HP} points!  (HP: {pokemon2.HP})");
                Thread.Sleep(1000);

                // Check if pokemon 2 is knocked out
                if (pokemon2.HP <= 0)
                {
                    Console.WriteLine($"\n{pokemon2.Name} is knocked out! {pokemon1.Name} wins!");
                    AwardExperience(pokemon1, 15);
                    break;
                }

                // Pokemon 2 attacks
                Console.WriteLine($"{pokemon2.Name} uses {pokemon2.Skill} for {pokemon2.SkillDamage} damage!");
                int originalHP1BeforeAttack = pokemon1.HP;
                pokemon1.CalculateDamage(pokemon2.SkillDamage);
                Console.WriteLine($"{pokemon1.Name} HP decreased by {originalHP1BeforeAttack - pokemon1.HP} points!  (HP: {pokemon1.HP})");
                Thread.Sleep(1000);

                // Check if pokemon 1 is knocked out
                if (pokemon1.HP <= 0)
                {
                    Console.WriteLine($"\n{pokemon1.Name} is knocked out! {pokemon2.Name} wins!");
                    AwardExperience(pokemon2, 15);
                    break;
                }

                // Pause between rounds
                if (round < 3)
                {
                    Console.WriteLine("\n--- Preparing for next round ---");
                    Thread.Sleep(1500);
                }
            }

            // Determine the winner if neither knocked out after 3 rounds
            if (pokemon1.HP > 0 && pokemon2.HP > 0)
            {
                double hp1Percent = (double)pokemon1.HP / originalHP1;
                double hp2Percent = (double)pokemon2.HP / originalHP2;

                if (hp1Percent > hp2Percent)
                {
                    Console.WriteLine($"\n{pokemon1.Name} wins by having more HP remaining!");
                    AwardExperience(pokemon1, 10);
                }
                else if (hp2Percent > hp1Percent)
                {
                    Console.WriteLine($"\n{pokemon2.Name} wins by having more HP remaining!");
                    AwardExperience(pokemon2, 10);
                }
                else
                {
                    Console.WriteLine("\nThe battle ends with a draw!");
                    AwardExperience(pokemon1, 5);
                    AwardExperience(pokemon2, 5);
                }
            }

            // Reset Pokémon HP after the battle
            pokemon1.HP = originalHP1;
            pokemon2.HP = originalHP2;

            Console.WriteLine("\nBattle completed! HP has been restored.");
        }

        // Helper method for battle experience
        private static void AwardExperience(Pokemon pokemon, int expAmount)
        {
            pokemon.Exp += expAmount;
            Console.WriteLine($"{pokemon.Name} gained {expAmount} EXP!");
        }

        // Create feature for training center
        private static void TrainPokemon(List<Pokemon> pokemonPocket)
        {
            Console.WriteLine("\n=== Pokemon Training Center ===");

            if (pokemonPocket.Count == 0)
            {
                Console.WriteLine("You don't have any Pokemon to train!");
                return;
            }

            // List available Pokemon
            Console.WriteLine("Choose a Pokemon to train:");
            for (int i = 0; i < pokemonPocket.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {pokemonPocket[i].Name} ({pokemonPocket[i].GetType().Name}, HP: {pokemonPocket[i].HP}, Exp: {pokemonPocket[i].Exp})");
            }

            Console.Write("\nSelect Pokemon (number): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > pokemonPocket.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            Pokemon selectedPokemon = pokemonPocket[choice - 1];

            Console.WriteLine("\n=== Training Options ===");
            Console.WriteLine("1. HP Training (Increase HP by 10)");
            Console.WriteLine("2. Experience Training (Gain 15 Exp)");
            Console.WriteLine("3. Special Training (Random improvement)");
            Console.Write("Choose training type: ");

            if (!int.TryParse(Console.ReadLine(), out int trainingType) ||
                trainingType < 1 || trainingType > 3)
            {
                Console.WriteLine("Invalid training type.");
                return;
            }

            // Animation for training
            Console.WriteLine("\nTraining in progress...");
            for (int i = 0; i < 5; i++)
            {
                Console.Write(".");
                Thread.Sleep(300);
            }
            Console.WriteLine("\n");

            // Apply training effects
            switch (trainingType)
            {
                case 1: // HP Training
                    int originalHP = selectedPokemon.HP;
                    selectedPokemon.HP += 10;
                    Console.WriteLine($"{selectedPokemon.Name}'s HP increased from {originalHP} to {selectedPokemon.HP}!");
                    break;

                case 2: // Experience Training
                    int originalExp = selectedPokemon.Exp;
                    selectedPokemon.Exp += 15;
                    Console.WriteLine($"{selectedPokemon.Name}'s Experience increased from {originalExp} to {selectedPokemon.Exp}!");
                    break;

                case 3: // Special Random Training
                    Random random = new Random();
                    int randomTraining = random.Next(1, 4);

                    switch (randomTraining)
                    {
                        case 1: // Big HP boost
                            originalHP = selectedPokemon.HP;
                            selectedPokemon.HP += 20;
                            Console.WriteLine($"SPECIAL TRAINING SUCCESS! {selectedPokemon.Name}'s HP got a major boost from {originalHP} to {selectedPokemon.HP}!");
                            break;

                        case 2: // Big Exp boost
                            originalExp = selectedPokemon.Exp;
                            selectedPokemon.Exp += 30;
                            Console.WriteLine($"SPECIAL TRAINING SUCCESS! {selectedPokemon.Name}'s Experience got a major boost from {originalExp} to {selectedPokemon.Exp}!");
                            break;

                        case 3: // Both HP and Exp boost
                            originalHP = selectedPokemon.HP;
                            originalExp = selectedPokemon.Exp;
                            selectedPokemon.HP += 10;
                            selectedPokemon.Exp += 10;
                            Console.WriteLine($"SPECIAL TRAINING SUCCESS! {selectedPokemon.Name} improved in both stats!");
                            Console.WriteLine($"HP: {originalHP} → {selectedPokemon.HP}");
                            Console.WriteLine($"Exp: {originalExp} → {selectedPokemon.Exp}");
                            break;
                    }
                    break;
            }

            Console.WriteLine("\nTraining complete!");
        }

        // CREATIVE FEATURE #3: Save/Load System
        private static void SaveLoadMenu(List<Pokemon> pokemonPocket)
        {
            Console.WriteLine("\n=== Save/Load System ===");
            Console.WriteLine("1. Save Pokemon to database");
            Console.WriteLine("2. Load Pokemon from database");
            Console.WriteLine("3. Export Pokemon to file");
            Console.WriteLine("4. Import Pokemon from file");
            Console.Write("Choose an option: ");
            
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > 4)
            {
                Console.WriteLine("Invalid option.");
                return;
            }
            
            switch (choice)
            {
                case 1: // Save to database
                    SavePokemonToDatabase(pokemonPocket);
                    break;
                case 2: // Load from database
                    LoadPokemonFromDatabase(pokemonPocket);
                    break;
                case 3: // Export to file
                    SavePokemonData(pokemonPocket);
                    break;
                case 4: // Import from file
                    LoadPokemonData(pokemonPocket);
                    break;
            }
        }

        private static void SavePokemonData(List<Pokemon> pokemonPocket)
        {
            try
            {
                // Animation
                Console.WriteLine("\nSaving your Pokemon data...");

                using (StreamWriter writer = new StreamWriter(SaveFilePath))
                {
                    // Write header
                    writer.WriteLine("Type,Name,HP,Exp");

                    // Write each Pokemon's data
                    foreach (Pokemon pokemon in pokemonPocket)
                    {
                        string type = pokemon.GetType().Name;
                        writer.WriteLine($"{type},{pokemon.Name},{pokemon.HP},{pokemon.Exp}");
                    }
                }

                Console.WriteLine($"Pokemon data successfully saved to {SaveFilePath}!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        private static void LoadPokemonData(List<Pokemon> pokemonPocket)
        {
            if (!File.Exists(SaveFilePath))
            {
                Console.WriteLine("\nNo save file found. Starting with an empty Pokemon pocket.");
                return;
            }

            try
            {
                Console.WriteLine("\nLoading Pokemon data...");

                // Clear current Pokemon list
                pokemonPocket.Clear();

                // Read save file
                string[] lines = File.ReadAllLines(SaveFilePath);

                // Skip header
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split(',');
                    if (data.Length != 4)
                    {
                        Console.WriteLine($"Skipping invalid line: {lines[i]}");
                        continue;
                    }

                    string type = data[0];
                    string name = data[1];
                    int hp = int.Parse(data[2]);
                    int exp = int.Parse(data[3]);

                    Pokemon pokemon = null;

                    switch (type)
                    {
                        case "Pikachu":
                            pokemon = new Pikachu(name, hp, exp);
                            break;
                        case "Eevee":
                            pokemon = new Eevee(name, hp, exp);
                            break;
                        case "Charmander":
                            pokemon = new Charmander(name, hp, exp);
                            break;
                        case "Raichu":
                            pokemon = new Raichu(name, hp, exp);
                            break;
                        case "Flareon":
                            pokemon = new Flareon(name, hp, exp);
                            break;
                        case "Charmeleon":
                            pokemon = new Charmeleon(name, hp, exp);
                            break;
                        default:
                            Console.WriteLine($"Unknown Pokemon type: {type}");
                            continue;
                    }

                    pokemonPocket.Add(pokemon);
                }

                Console.WriteLine($"Successfully loaded {pokemonPocket.Count} Pokemon from save file!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        // Evolution Animation
        private static void ShowEvolutionAnimation()
        {
            Console.WriteLine("\nEvolution in progress:");

            string[] evolutionFrames = {
                "[ * * * ]",
                "[* * * *]",
                "[ * * * ]",
                "[* * * *]",
                "[**EVOLVING**]"
            };

            foreach (string frame in evolutionFrames)
            {
                Console.WriteLine(frame);
                Thread.Sleep(500);
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine(new string(' ', frame.Length));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }

            Console.WriteLine("EVOLUTION COMPLETE!");
            Thread.Sleep(1000);
        }

        // Method to initialize the database
        private static void InitializeDatabase(List<PokemonMaster> pokemonMasters)
        {
            try
            {
                // Create the Database directory if it doesn't exist
                Directory.CreateDirectory(DatabaseDirectory);
                
                // Create connection string
                string connectionString = $"Data Source={DatabasePath}";
                
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    
                    // Create PokemonMasters table
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS PokemonMasters (
                                Id INTEGER PRIMARY KEY,
                                Name TEXT NOT NULL,
                                NoToEvolve INTEGER NOT NULL,
                                EvolveTo TEXT NOT NULL
                            )";
                        command.ExecuteNonQuery();
                    }
                    
                    // Create Pokemons table
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Pokemons (
                                Id INTEGER PRIMARY KEY,
                                Name TEXT NOT NULL,
                                Type TEXT NOT NULL,
                                HP INTEGER NOT NULL,
                                Exp INTEGER NOT NULL,
                                Skill TEXT NOT NULL,
                                SkillDamage INTEGER NOT NULL
                            )";
                        command.ExecuteNonQuery();
                    }
                    
                    // Check if PokemonMasters has data
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT COUNT(*) FROM PokemonMasters";
                        long count = (long)command.ExecuteScalar();
                        
                        // If no data exists, seed the table
                        if (count == 0)
                        {
                            using (var transaction = connection.BeginTransaction())
                            {
                                using (var insertCommand = connection.CreateCommand())
                                {
                                    insertCommand.CommandText = "INSERT INTO PokemonMasters (Name, NoToEvolve, EvolveTo) VALUES (@Name, @NoToEvolve, @EvolveTo)";
                                    
                                    // Parameters need to be recreated for each execution
                                    var nameParam = insertCommand.CreateParameter();
                                    nameParam.ParameterName = "@Name";
                                    insertCommand.Parameters.Add(nameParam);
                                    
                                    var noToEvolveParam = insertCommand.CreateParameter();
                                    noToEvolveParam.ParameterName = "@NoToEvolve";
                                    insertCommand.Parameters.Add(noToEvolveParam);
                                    
                                    var evolveToParam = insertCommand.CreateParameter();
                                    evolveToParam.ParameterName = "@EvolveTo";
                                    insertCommand.Parameters.Add(evolveToParam);
                                    
                                    // Insert each Pokemon master
                                    foreach (var master in pokemonMasters)
                                    {
                                        nameParam.Value = master.Name;
                                        noToEvolveParam.Value = master.NoToEvolve;
                                        evolveToParam.Value = master.EvolveTo;
                                        insertCommand.ExecuteNonQuery();
                                    }
                                }
                                transaction.Commit();
                            }
                            Console.WriteLine("Pokemon evolution data initialized successfully!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        // Method to load Pokemon from the database
        private static void LoadPokemonFromDatabase(List<Pokemon> pokemonPocket)
        {
            try
            {
                using (var db = new PokemonDbContext())
                {
                    // Clear current pocket
                    pokemonPocket.Clear();

                    // Fetch all Pokemon from database
                    var pokemonEntities = db.Pokemons.ToList();

                    if (pokemonEntities.Count == 0)
                    {
                        Console.WriteLine("No Pokemon found in database.");
                        return;
                    }

                    // Convert database entities to Pokemon objects
                    foreach (var entity in pokemonEntities)
                    {
                        Pokemon pokemon = null;

                        switch (entity.Type)
                        {
                            case "Pikachu":
                                pokemon = new Pikachu(entity.Name, entity.HP, entity.Exp);
                                break;
                            case "Eevee":
                                pokemon = new Eevee(entity.Name, entity.HP, entity.Exp);
                                break;
                            case "Charmander":
                                pokemon = new Charmander(entity.Name, entity.HP, entity.Exp);
                                break;
                            case "Raichu":
                                pokemon = new Raichu(entity.Name, entity.HP, entity.Exp);
                                break;
                            case "Flareon":
                                pokemon = new Flareon(entity.Name, entity.HP, entity.Exp);
                                break;
                            case "Charmeleon":
                                pokemon = new Charmeleon(entity.Name, entity.HP, entity.Exp);
                                break;
                        }

                        if (pokemon != null)
                        {
                            pokemonPocket.Add(pokemon);
                        }
                    }

                    Console.WriteLine($"Successfully loaded {pokemonPocket.Count} Pokemon from database!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Pokemon from database: {ex.Message}");
            }
        }

        // Method to save Pokemon to the database
        private static void SavePokemonToDatabase(List<Pokemon> pokemonPocket)
        {
            try
            {
                using (var db = new PokemonDbContext())
                {
                    // Clear existing Pokemon data
                    db.Pokemons.RemoveRange(db.Pokemons);

                    // Add all current Pokemon to database
                    foreach (var pokemon in pokemonPocket)
                    {
                        db.Pokemons.Add(new PokemonEntity
                        {
                            Name = pokemon.Name,
                            Type = pokemon.GetType().Name,
                            HP = pokemon.HP,
                            Exp = pokemon.Exp,
                            Skill = pokemon.Skill,
                            SkillDamage = pokemon.SkillDamage
                        });
                    }

                    // Save changes
                    db.SaveChanges();
                    Console.WriteLine($"Successfully saved {pokemonPocket.Count} Pokemon to database!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving Pokemon to database: {ex.Message}");
            }
        }
    }
}

