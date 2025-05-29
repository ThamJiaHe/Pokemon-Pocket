using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.IO;
using System.Data;
using Microsoft.Data.Sqlite;
using PokemonPocket.Database;
// Author: THAM JIA HE (244548T)
// Date: 2025-05-10
// Copyright (c) 2025 THAM JIA HE. All rights reserved.
/*
AI Acknowledgment:
Parts of this project were developed with assistance from Copilot, an AI programming assistant. 
Specifically, AI was used to:
- Help implement the color coding for battle messages
- Debug issues with the battle mechanics
- Suggest improvements for database operations
- Provide feedback on code structure and organization

All core implementation, design decisions, and final code review were completed by me 
(THAM JIA HE, 244548T). The AI was used as a programming assistant only, with all generated
code being reviewed, understood, and modified by me before inclusion.
*/


// The information to create a Pokémon Pocket Console Application was inspired by the game *Pokemmo* I played when I was a kid and the scenario of the Assignment.

/* To Create a Pokémon Pocket Console Application that allows Pokémon’s player:
1. Add Pokémon to the pocket
2. List Pokémon in the pocket in descending order of experience points (Exp)
3. Check if Pokémon can evolve
4. Evolve Pokémon  
*/

/*
Additional Features I added to spice up the game and make it more fun and realistic:
1. Battle Arena: Allow players to battle their Pokémon against each other or against AI and choose their moves like attack or defend.
2. Training Center: Allow players to train their Pokémon to increase HP and EXP.
3. Save & Load: Save Pokémon data to a database using SQLite or CSV files and load the game where they left off.
4. Colourful Console Output: Use different colors for success and error messages to enhance user experience.
*/

/*
The reason why I save the Pokémon data to CSV files apart from the database is to allow players to play on different devices or share their Pokémon data easily.
I also added a database to store the Pokémon data, which allows for easy retrieval and management of Pokémon information.
*/
/*
Technical Implementation Overview:
- Object-oriented design with inheritance hierarchy for Pokémon types
- SQLite database integration for persistent storage
- Type-based battle mechanics with effectiveness multipliers
- Intelligent AI opponent for single-player battles
- Evolution system that transforms Pokémon into stronger forms
*/

// This is the main namespace for the Pokémon Pocket application  
namespace PokemonPocket
{
    class Program
    {
        private static readonly string SaveFilePath = "pokemon_save.csv";
        private static readonly string DatabaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
        private static readonly string DatabasePath = Path.Combine(DatabaseDirectory, "pokemon_pocket.db");

        static void Main(string[] args)
        {
            // Setup evolution information - this defines which Pokemon can evolve and what they evolve into
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
                Console.Write("Please only enter [1,2,3,4,5,6,7] or Q to quit: ");

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
                            PauseBeforeReturningToMenu(); // Add this line
                            break;
                        // Option 2: List all Pokémon in the pocket
                        case '2':
                            ListPokemon(pokemonPocket);
                            PauseBeforeReturningToMenu(); // Add this line
                            break;
                        // Option 3: Check if I can evolve Pokémon
                        case '3':
                            CheckEvolution(pokemonPocket, pokemonMasters);
                            PauseBeforeReturningToMenu(); // Add this line
                            break;
                        // Option 4: Evolve Pokémon if possible
                        case '4':
                            EvolvePokemon(pokemonPocket, pokemonMasters);
                            PauseBeforeReturningToMenu(); // Add this line
                            break;
                        // Option 5: Battle Arena
                        case '5':
                            BattleArena(pokemonPocket);
                            PauseBeforeReturningToMenu(); // Add this line
                            break;
                        // Option 6: Training Center
                        case '6':
                            TrainPokemon(pokemonPocket);
                            PauseBeforeReturningToMenu(); // Add this line
                            break;
                        // Option 7: Save & Load
                        case '7':
                            SaveLoadMenu(pokemonPocket);
                            PauseBeforeReturningToMenu(); // Add this line
                            break;
                        // Exit the program if the user enters 'q'Lowercase or 'Q'Uppercase
                        case 'q':
                        case 'Q':
                            Console.WriteLine("Saving your Pokémon data before exiting...");
                            try
                            {
                                // Make sure the directory exists before saving
                                Directory.CreateDirectory(DatabaseDirectory);
                                SavePokemonToDatabase(pokemonPocket);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Couldn't save data: {ex.Message}");
                            }
                            Console.WriteLine("Thank you for using Pokémon Pocket! Goodbye!");
                            running = false;
                            Environment.Exit(0); // Ensure clean exit
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

        // Adding Pokémon to the Player's pocket
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

                // Prompt for pokemon type explicitly instead of determining from name
                Console.WriteLine("Enter Pokémon type (Pikachu, Eevee, or Charmander):");
                string pokemonType = Console.ReadLine()?.Trim();

                // Validate the type is one of the allowed types
                if (string.IsNullOrWhiteSpace(pokemonType) ||
                    (pokemonType.ToLower() != "pikachu" &&
                     pokemonType.ToLower() != "eevee" &&
                     pokemonType.ToLower() != "charmander"))
                {
                    Console.WriteLine("Invalid Pokémon type. Please enter Pikachu, Eevee, or Charmander.");
                    return;
                }

                // Prompt the user for Pokémon HP
                Console.WriteLine("Enter Pokémon's HP: ");
                string hpInput = Console.ReadLine();

                // Validate the input to ensure it's a number
                if (!int.TryParse(hpInput, out int hp) || hp <= 0)
                {
                    Console.WriteLine("Invalid HP. Please enter a positive number.");
                    return;
                }

                // Prompt the user for Pokémon EXP
                Console.WriteLine("Enter Pokémon's EXP: ");
                string expInput = Console.ReadLine();

                if (!int.TryParse(expInput, out int exp) || exp <= 0)
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
                // Set text color to green for success messages
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Pokemon successfully added!");
                Console.ResetColor(); // Reset to default color
            }
            catch (Exception ex)
            {
                // Set text color to red for error messages
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Invalid selection");
                Console.ResetColor();
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Determine Pokémon type based on the name
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

        // Display all Pokemon in the Player pocket in descending order of experience points (Exp)
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

        // This checks against the evolution requirements in pokemonMasters list
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

        // Evolve the Pokemon to a stronger form
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
                    // Add dramatic evolution animation
                    Console.WriteLine($"\nPreparing to evolve {master.Name}...");
                    Thread.Sleep(1000);

                    // Sort by lowest exp first to evolve those
                    eligiblePokemon = eligiblePokemon.OrderBy(p => p.Exp).ToList();

                    // Show Pokemon stats before evolution
                    Pokemon toEvolve = eligiblePokemon[0];
                    Console.WriteLine($"\nBefore evolution:");
                    Console.WriteLine($"Name: {toEvolve.Name}");
                    Console.WriteLine($"Type: {toEvolve.GetType().Name}");
                    Console.WriteLine($"HP: {toEvolve.HP}");
                    Console.WriteLine($"Exp: {toEvolve.Exp}");
                    Console.WriteLine($"Skill: {toEvolve.Skill} (Power: {toEvolve.SkillDamage})");

                    // Calculate stat improvements (higher HP and retained EXP)
                    int newHP = toEvolve.HP + 50; // Evolution boosts HP
                    int retainedExp = toEvolve.Exp;

                    // Remove the Pokemon that will be evolved
                    for (int i = 0; i < master.NoToEvolve; i++)
                    {
                        Pokemon toRemove = eligiblePokemon[i];
                        pokemonPocket.Remove(toRemove);
                    }

                    // Show evolution animation
                    ShowEvolutionAnimation();

                    // Create the evolved Pokemon
                    Pokemon evolvedPokemon = null;

                    switch (master.EvolveTo)
                    {
                        case "Raichu":
                            evolvedPokemon = new Raichu(toEvolve.Name, newHP, retainedExp);
                            break;
                        case "Flareon":
                            evolvedPokemon = new Flareon(toEvolve.Name, newHP, retainedExp);
                            break;
                        case "Charmeleon":
                            evolvedPokemon = new Charmeleon(toEvolve.Name, newHP, retainedExp);
                            break;
                    }

                    // Show stats after evolution
                    Console.WriteLine($"\nAfter evolution:");
                    Console.WriteLine($"Name: {evolvedPokemon.Name}");
                    Console.WriteLine($"Type: {evolvedPokemon.GetType().Name}");
                    Console.WriteLine($"HP: {evolvedPokemon.HP}");
                    Console.WriteLine($"Exp: {evolvedPokemon.Exp}");
                    Console.WriteLine($"Skill: {evolvedPokemon.Skill} (Power: {evolvedPokemon.SkillDamage})");

                    // Add the evolved Pokemon to the pocket
                    pokemonPocket.Add(evolvedPokemon);
                    Console.WriteLine($"\n{master.Name} evolved into {master.EvolveTo}!");
                    anyEvolved = true;
                }
            }

            // If no Pokemon were evolved, inform the user
            if (!anyEvolved)
            {
                Console.WriteLine("You don't have enough Pokemon to evolve.");
            }
        }

        // like the actual game it allows Pokemon to fight against each other but with a added twist of AI opponents
        // Implements turn-based combat with attack/defend options and type effectiveness
        private static void BattleArena(List<Pokemon> pokemonPocket)
        {
            Console.WriteLine("Welcome to the Battle Arena!");

            if (pokemonPocket.Count < 2)
            {
                Console.WriteLine("You need at least 2 Pokémon to battle.");
                return;
            }

            // Let player choose battle mode
            Console.WriteLine("\nBattle Mode:");
            Console.WriteLine("1. Player vs Player");
            Console.WriteLine("2. Player vs Computer AI");
            Console.Write("Select mode: ");

            if (!int.TryParse(Console.ReadLine(), out int modeChoice) || (modeChoice != 1 && modeChoice != 2))
            {
                Console.WriteLine("Invalid selection. Defaulting to Player vs Player.");
                modeChoice = 1;
            }

            bool vsAI = (modeChoice == 2);

            // List available Pokémon
            Console.WriteLine("\nChoose your Pokémon to battle: ");
            for (int i = 0; i < pokemonPocket.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {pokemonPocket[i].Name} {pokemonPocket[i].GetType().Name}, HP: {pokemonPocket[i].HP}, EXP: {pokemonPocket[i].Exp}");
            }

            // Player 1 selects Pokémon
            Console.Write("\nPlayer 1, select your Pokémon (number): ");
            if (!int.TryParse(Console.ReadLine(), out int choice1) || choice1 < 1 || choice1 > pokemonPocket.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            // Get Player 1's Pokémon
            Pokemon pokemon1 = pokemonPocket[choice1 - 1];

            // Player 2 or AI selects Pokémon
            Pokemon pokemon2;

            if (!vsAI)
            {
                // Player 2 selects Pokémon
                Console.Write("Player 2, select your Pokémon (number): ");
                if (!int.TryParse(Console.ReadLine(), out int choice2) ||
                    choice2 < 1 || choice2 > pokemonPocket.Count ||
                    choice1 == choice2)
                {
                    Console.WriteLine("Invalid selection.");
                    return;
                }

                pokemon2 = pokemonPocket[choice2 - 1];
            }
            else
            {
                // AI selects Pokémon (randomly or strategically)
                Random random = new Random();
                List<Pokemon> aiChoices = new List<Pokemon>();

                // Exclude player's Pokémon from AI choices
                for (int i = 0; i < pokemonPocket.Count; i++)
                {
                    if (i != choice1 - 1)
                    {
                        aiChoices.Add(pokemonPocket[i]);
                    }
                }

                if (aiChoices.Count == 0)
                {
                    Console.WriteLine("No available Pokémon for the AI to choose.");
                    return;
                }

                // AI makes a strategic choice (selects Pokémon with type advantage if possible)
                // For simplicity, we'll just choose randomly for now
                int aiChoice = random.Next(aiChoices.Count);
                pokemon2 = aiChoices[aiChoice];

                Console.WriteLine($"AI has chosen {pokemon2.Name} ({pokemon2.GetType().Name})!");
            }

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

                // Player 1's turn
                string player1Move = GetPlayerMove(pokemon1, 1);

                // Player 2 or AI's turn
                string player2Move;
                if (!vsAI)
                {
                    player2Move = GetPlayerMove(pokemon2, 2);
                }
                else
                {
                    // AI selects a move (for now, always attacks)
                    player2Move = GetAIMove(pokemon2, pokemon1);
                    if (player2Move == "attack")
                        Console.WriteLine($"AI chooses to use {pokemon2.Skill}!");
                    else
                        Console.WriteLine($"AI chooses to defend with {pokemon2.Name}!");
                }

                // Determine who goes first based on Pokémon's Exp (simulating Speed stat)
                bool player1First = pokemon1.Exp >= pokemon2.Exp;

                if (player1First)
                {
                    Console.WriteLine($"{pokemon1.Name} moves first due to higher experience!");

                    // Execute move and track if defending
                    bool isDefending1 = ExecuteMove(pokemon1, pokemon2, player1Move, 1);
                    pokemon1.IsDefending = isDefending1;

                    // Check if Pokemon 2 fainted
                    if (pokemon2.HP <= 0)
                    {
                        Console.WriteLine($"\n{pokemon2.Name} is knocked out! {pokemon1.Name} wins!");
                        AwardExperience(pokemon1, 15);
                        break;
                    }

                    // Apply defense bonus if defending
                    if (pokemon2.IsDefending)
                    {
                        Console.WriteLine($"{pokemon2.Name}'s defensive stance reduces damage!");
                        // Execute move with defense consideration
                        ExecuteMoveWithDefense(pokemon2, pokemon1, player2Move, vsAI ? 0 : 2);
                        pokemon2.IsDefending = false; // Reset defense status
                    }
                    else
                    {
                        ExecuteMove(pokemon2, pokemon1, player2Move, vsAI ? 0 : 2);
                    }

                    // Check if Pokemon 1 fainted
                    if (pokemon1.HP <= 0)
                    {
                        Console.WriteLine($"\n{pokemon1.Name} is knocked out! {pokemon2.Name} wins!");
                        AwardExperience(pokemon2, 15);
                        break;
                    }
                }
                else
                {
                    Console.WriteLine($"{pokemon2.Name} moves first due to higher experience!");
                    ExecuteMove(pokemon2, pokemon1, player2Move, vsAI ? 0 : 2); // 0 for AI

                    // Check if Pokemon 1 fainted
                    if (pokemon1.HP <= 0)
                    {
                        Console.WriteLine($"\n{pokemon1.Name} is knocked out! {pokemon2.Name} wins!");
                        AwardExperience(pokemon2, 15);
                        break;
                    }

                    ExecuteMove(pokemon1, pokemon2, player1Move, 1);

                    // Check if Pokemon 2 fainted
                    if (pokemon2.HP <= 0)
                    {
                        Console.WriteLine($"\n{pokemon2.Name} is knocked out! {pokemon1.Name} wins!");
                        AwardExperience(pokemon1, 15);
                        break;
                    }
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
                    // When a Pokémon wins
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n{pokemon1.Name} wins by having more HP remaining!");
                    Console.ResetColor();
                    AwardExperience(pokemon1, 10);
                }
                else if (hp2Percent > hp1Percent)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n{pokemon2.Name} wins by having more HP remaining!");
                    Console.ResetColor();
                    AwardExperience(pokemon2, 15);
                }
                else
                {
                    // For a draw
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nThe battle ends with a draw!");
                    Console.ResetColor();
                    AwardExperience(pokemon1, 5);
                    AwardExperience(pokemon2, 5);
                }
            }

            // Reset Pokémon HP after the battle
            pokemon1.HP = originalHP1;
            pokemon2.HP = originalHP2;

            Console.WriteLine("\nBattle completed! HP has been restored.");
        }

        // Helper method to get player move choice
        private static string GetPlayerMove(Pokemon pokemon, int playerNumber)
        {
            Console.WriteLine($"\nPlayer {playerNumber}'s turn with {pokemon.Name}");
            Console.WriteLine($"1. Attack with {pokemon.Skill} (Damage: {pokemon.SkillDamage})");
            Console.WriteLine("2. Defend (reduces damage from next attack)");
            Console.Write("Choose your move: ");

            if (!int.TryParse(Console.ReadLine(), out int moveChoice) || (moveChoice != 1 && moveChoice != 2))
            {
                Console.WriteLine("Invalid choice. Defaulting to attack.");
                moveChoice = 1;
            }

            return moveChoice == 1 ? "attack" : "defend";
        }

        // Execute a move
        private static bool ExecuteMove(Pokemon attacker, Pokemon defender, string moveType, int playerNumber)
        {
            string playerLabel = playerNumber == 0 ? "AI" : $"Player {playerNumber}";

            if (moveType == "attack")
            {
                bool isCritical = IsCriticalHit();
                double effectiveness = GetTypeEffectiveness(attacker.GetType().Name, defender.GetType().Name);
                double damageMultiplier = effectiveness * (isCritical ? 2.0 : 1.0);

                Console.WriteLine($"{playerLabel}: {attacker.Name} uses {attacker.Skill} for {attacker.SkillDamage} base damage!");

                // For critical hits
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("A critical hit!");
                Console.ResetColor();

                // For super effective moves
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("It's super effective!");
                Console.ResetColor();

                // For not very effective moves
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("It's not very effective...");
                Console.ResetColor();

                int originalDefenderHP = defender.HP;
                defender.CalculateDamage((int)(attacker.SkillDamage * damageMultiplier));
                int damageDone = originalDefenderHP - defender.HP;

                // Create effectiveness text based on the effectiveness value
                string effectivenessText = effectiveness > 1.0 ? "It's super effective!" :
                                (effectiveness < 1.0 ? "It's not very effective..." : "");

                if (!string.IsNullOrEmpty(effectivenessText))
                    Console.WriteLine(effectivenessText);

                Console.WriteLine($"{defender.Name} HP decreased by {damageDone} points! (HP: {defender.HP})");

                return false; // Not defending
            }
            else // defend
            {
                // For defense stance
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{playerLabel}: {attacker.Name} takes a defensive stance!");
                Console.ResetColor();
                return true; // Is defending
            }

        }

        // Trains the Pokémon in the Training Center
        private static void TrainPokemon(List<Pokemon> pokemonPocket)
        {
            Console.WriteLine("\n=== Pokémon Training Center ===");

            if (pokemonPocket.Count == 0)
            {
                Console.WriteLine("You don't have any Pokémon to train!");
                return;
            }

            // List available Pokemon
            Console.WriteLine("Choose a Pokémon to train:");
            for (int i = 0; i < pokemonPocket.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {pokemonPocket[i].Name} ({pokemonPocket[i].GetType().Name})");
                Console.WriteLine($"   HP: {pokemonPocket[i].HP}, Exp: {pokemonPocket[i].Exp}");
                Console.WriteLine($"   Skill: {pokemonPocket[i].Skill} (Power: {pokemonPocket[i].SkillDamage})");
            }

            Console.Write("\nSelect Pokémon (number): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > pokemonPocket.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            Pokemon selectedPokemon = pokemonPocket[choice - 1];

            // Training options that reflect actual Pokémon games
            Console.WriteLine("\n=== Training Facilities ===");
            Console.WriteLine("1. HP Training (Gains +10 HP)");
            Console.WriteLine("2. Battle Training (Gains +15 EXP)");
            Console.WriteLine("3. Elite Four Challenge (Random stat boost, higher risk/reward)");
            Console.Write("Choose training type: ");

            if (!int.TryParse(Console.ReadLine(), out int trainingType) ||
                trainingType < 1 || trainingType > 3)
            {
                Console.WriteLine("Invalid training type.");
                return;
            }

            // Training progress animation
            AnimateTraining(trainingType);

            // Apply training effects
            switch (trainingType)
            {
                case 1: // HP Training
                    int originalHP = selectedPokemon.HP;
                    selectedPokemon.HP += 10;
                    Console.WriteLine($"\n{selectedPokemon.Name} completed HP training!");
                    Console.WriteLine($"HP: {originalHP} → {selectedPokemon.HP} (+10)");
                    break;

                case 2: // Battle Training
                    int originalExp = selectedPokemon.Exp;
                    selectedPokemon.Exp += 15;
                    Console.WriteLine($"\n{selectedPokemon.Name} completed battle training!");
                    Console.WriteLine($"EXP: {originalExp} → {selectedPokemon.Exp} (+15)");
                    break;

                case 3: // Elite Four Challenge - Higher risk/reward
                    Random random = new Random();
                    int result = random.Next(1, 101); // 1-100

                    if (result <= 20)
                    { // 20% chance of failure
                        Console.WriteLine($"\n{selectedPokemon.Name} was defeated by the Elite Four!");
                        Console.WriteLine("No stats were improved, but it was a good learning experience.");
                    }
                    else if (result <= 80)
                    { // 60% chance of decent improvement
                        originalHP = selectedPokemon.HP;
                        originalExp = selectedPokemon.Exp;
                        int hpBoost = random.Next(5, 16); // 5-15
                        int expBoost = random.Next(10, 21); // 10-20

                        selectedPokemon.HP += hpBoost;
                        selectedPokemon.Exp += expBoost;

                        Console.WriteLine($"\n{selectedPokemon.Name} performed well against the Elite Four!");
                        Console.WriteLine($"HP: {originalHP} → {selectedPokemon.HP} (+{hpBoost})");
                        Console.WriteLine($"EXP: {originalExp} → {selectedPokemon.Exp} (+{expBoost})");
                    }
                    else
                    { // 20% chance of excellent improvement
                        originalHP = selectedPokemon.HP;
                        originalExp = selectedPokemon.Exp;
                        int hpBoost = random.Next(15, 26); // 15-25
                        int expBoost = random.Next(20, 41); // 20-40

                        selectedPokemon.HP += hpBoost;
                        selectedPokemon.Exp += expBoost;

                        Console.WriteLine($"\n{selectedPokemon.Name} DEFEATED the Elite Four!");
                        Console.WriteLine($"HP: {originalHP} → {selectedPokemon.HP} (+{hpBoost})");
                        Console.WriteLine($"EXP: {originalExp} → {selectedPokemon.Exp} (+{expBoost})");
                    }
                    break;
            }

            Console.WriteLine("\nTraining complete!");
        }
        // Visual animation for training to improve user experience
        // Shows progress through different training phases        
        private static void AnimateTraining(int trainingType)
        {
            Console.WriteLine("\nTraining in progress...");
            string[] animations = {
                "Warming up...",
                "Building strength...",
                "Practicing techniques...",
                "Pushing limits...",
                "Cooling down..."
            };

            foreach (string phase in animations)
            {
                Console.Write(phase);
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    Console.Write(".");
                }
                Console.WriteLine();
            }
        }

        // Allows saving/loading from both database and CSV files
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
            // No need for PauseBeforeReturningToMenu() here since we call it after SaveLoadMenu in the main menu
        }

        // Saves Pokemon data to CSV file for portability
        private static void SavePokemonData(List<Pokemon> pokemonPocket)
        {
            try
            {
                // Animation
                Console.WriteLine("\nSaving your Pokemon data...");

                using (StreamWriter writer = new StreamWriter(SaveFilePath))
                {
                    // Write header
                    writer.WriteLine("Name,Type,HP,Exp");

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

        // Loads Pokemon data from CSV file
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

        // Sets up the SQLite database for storing Pokemon data
        // Creates tables if they don't exist and seeds evolution data
        private static void InitializeDatabase(List<PokemonMaster> pokemonMasters)
        {
            try
            {
                // Make sure the database directory exists
                Directory.CreateDirectory(DatabaseDirectory);

                using (var db = new PokemonDbContext(DatabasePath))
                {
                    // Check if database has been seeded already
                    if (!db.EnsureCreated())
                    {
                        // Seed the database with PokemonMaster data
                        db.SeedPokemonMasters(pokemonMasters);
                        Console.WriteLine("Database initialized with Pokemon evolution data.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        // Loads Pokemon from SQLite database into memory
        // This allows the player to load their last saved Pokemon game play session. 
        private static void LoadPokemonFromDatabase(List<Pokemon> pokemonPocket)
        {
            try
            {
                using (var db = new PokemonDbContext(DatabasePath))
                {
                    // Clear the current pocket
                    pokemonPocket.Clear();

                    // Create Pokemon objects from entities
                    foreach (var entity in db.Pokemons)
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

        // Saves current Pokemon collection to SQLite database
        private static void SavePokemonToDatabase(List<Pokemon> pokemonPocket)
        {
            try
            {
                using (var db = new PokemonDbContext(DatabasePath))
                {
                    // Clear existing Pokemon
                    db.Pokemons.Clear();

                    // Add current Pokemon to context
                    foreach (var pokemon in pokemonPocket)
                    {
                        db.Add(new PokemonEntity
                        {
                            Name = pokemon.Name,
                            Type = pokemon.GetType().Name,
                            HP = pokemon.HP,
                            Exp = pokemon.Exp,
                            Skill = pokemon.Skill,
                            SkillDamage = pokemon.SkillDamage
                        });
                    }

                    // Save changes to database
                    db.SaveChanges();

                    Console.WriteLine($"Successfully saved {pokemonPocket.Count} Pokemon to database!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving Pokemon to database: {ex.Message}");
            }
        }

        // Visual animation for evolution to make the experience more engaging
        // Mimics the flashing animation from the original Pokemon games
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
        // Awards experience points to Pokemon after battles
        // Also notifies player if Pokemon is ready for evolutio
        private static void AwardExperience(Pokemon pokemon, int expAmount)
        {
            int originalExp = pokemon.Exp;
            pokemon.Exp += expAmount;

            Console.WriteLine($"{pokemon.Name} gained {expAmount} experience points!");
            Console.WriteLine($"EXP: {originalExp} → {pokemon.Exp}");

            // Optional: Check if the Pokémon would be eligible for evolution after gaining exp
            // This could encourage the player to use the evolution feature
            if (pokemon.GetType().Name == "Pikachu" && originalExp < 50 && pokemon.Exp >= 50)
            {
                Console.WriteLine($"Your {pokemon.Name} seems ready for evolution! Check the evolution menu.");
            }
            else if (pokemon.GetType().Name == "Eevee" && originalExp < 40 && pokemon.Exp >= 40)
            {
                Console.WriteLine($"Your {pokemon.Name} seems ready for evolution! Check the evolution menu.");
            }
            else if (pokemon.GetType().Name == "Charmander" && originalExp < 30 && pokemon.Exp >= 30)
            {
                Console.WriteLine($"Your {pokemon.Name} seems ready for evolution! Check the evolution menu.");
            }
        }

        // Gives the player time to read messages before returning to the menu
        private static void PauseBeforeReturningToMenu()
        {
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey(true);
            Console.Clear(); // Optional: clear the screen for a cleaner interface
        }

        // Implements a simplified version of the type effectiveness chart from Pokemon games
        private static double GetTypeEffectiveness(string attackerType, string defenderType)
        {
            // Basic type matchups (simplified from actual Pokémon games)
            if (attackerType == "Pikachu" && defenderType == "Charmander")
                return 1.5; // Electric is strong against Fire
            if (attackerType == "Charmander" && defenderType == "Eevee")
                return 1.5; // Fire is strong against Normal
            if (attackerType == "Eevee" && defenderType == "Pikachu")
                return 1.5; // Normal is strong against Electric

            // For evolved forms
            if (attackerType == "Raichu" && (defenderType == "Charmander" || defenderType == "Charmeleon"))
                return 1.5;
            if ((attackerType == "Charmeleon" || attackerType == "Charmander") &&
                (defenderType == "Eevee" || defenderType == "Flareon"))
                return 1.5;
            if ((attackerType == "Eevee" || attackerType == "Flareon") &&
                (defenderType == "Pikachu" || defenderType == "Raichu"))
                return 1.5;

            return 1.0; // Default - normal effectiveness
        }
        // Enhanced AI decision making
        private static string GetAIMove(Pokemon aiPokemon, Pokemon playerPokemon)
        {
            Random random = new Random();

            // If AI's HP is low (below 30%), it has a higher chance to defend
            if (aiPokemon.HP < 30 && random.Next(100) < 70)
            {
                return "defend";
            }

            // If player's HP is low, AI is more likely to attack to finish them
            if (playerPokemon.HP < 20 && random.Next(100) < 90)
            {
                return "attack";
            }

            // Otherwise, AI has a 20% chance to defend, 80% to attack
            return random.Next(100) < 20 ? "defend" : "attack";
        }

        // Determines if an attack is a critical hit (10% chance)   
        // Adds an element of luck to battles, just like in the original games    
        private static bool IsCriticalHit()
        {
            Random random = new Random();
            return random.Next(100) < 10; // 10% chance of critical hit
        }

        // Add this method to handle attacks when the defender is defending
        // Reduces damage by 50% to make the defend option worthwhile
        private static void ExecuteMoveWithDefense(Pokemon attacker, Pokemon defender, string moveType, int playerNumber)
        {
            string playerLabel = playerNumber == 0 ? "AI" : $"Player {playerNumber}";

            if (moveType == "attack")
            {
                bool isCritical = IsCriticalHit();
                double effectiveness = GetTypeEffectiveness(attacker.GetType().Name, defender.GetType().Name);

                // Defense reduces damage by 50%
                double defenseModifier = 0.5;
                double damageMultiplier = effectiveness * (isCritical ? 2.0 : 1.0) * defenseModifier;

                Console.WriteLine($"{playerLabel}: {attacker.Name} uses {attacker.Skill} for {attacker.SkillDamage} base damage!");

                if (isCritical)
                    Console.WriteLine("A critical hit!");

                int originalDefenderHP = defender.HP;
                defender.CalculateDamage((int)(attacker.SkillDamage * damageMultiplier));
                int damageDone = originalDefenderHP - defender.HP;

                // Create effectiveness text based on the effectiveness value
                string effectivenessText = effectiveness > 1.0 ? "It's super effective!" :
                                (effectiveness < 1.0 ? "It's not very effective..." : "");

                if (!string.IsNullOrEmpty(effectivenessText))
                    Console.WriteLine(effectivenessText);

                Console.WriteLine($"Defense reduced the damage! {defender.Name} HP decreased by only {damageDone} points! (HP: {defender.HP})");
            }
            else
            {
                Console.WriteLine($"{playerLabel}: {attacker.Name} is already defending and chooses to reinforce their defense!");
                // If they choose to defend while already defending, nothing happens
            }
        }
    }
}

