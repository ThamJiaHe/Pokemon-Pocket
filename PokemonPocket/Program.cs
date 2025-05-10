using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PokemonPocket
{
    class Program
    {
        static void Main(string[] args)
        {
            // This list holds the evolution information for Pokémon
            // Each entry contains the Pokémon name, the number of Pokémon needed to evolve, and the evolved form
            List<PokemonMaster> pokemonMasters = new List<PokemonMaster>(){
            new PokemonMaster("Pikachu", 2, "Raichu"),
            new PokemonMaster("Eevee", 3, "Flareon"),
            new PokemonMaster("Charmander", 1, "Charmeleon")
            };

            //Use "Environment.Exit(0);" if you want to implement an exit of the console program

            //Start your assignment 1 requirements below.
            //Create a list to hold Pokemon objects
            List<Pokemon> pokemonPocket = new List<Pokemon>();

            // Variable to hold the user's choice
            char choice;
            // Create a boolean variable to control the loop
            bool running = true;

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
                Console.Write("Please only enter [1,2,3,4] or Q to quit: ");

                // Read user input with validation
                try
                {
                    // Read the user's choice
                    choice = Console.ReadKey(true).KeyChar;
                    Console.WriteLine(choice);

                    // Check if the choice is valid
                    switch (choice)
                    {
                        // Add a new Pokémon to the pocket
                        case '1':
                            AddPokemon(pokemonPocket);
                            break;
                        // List all Pokémon in the pocket
                        case '2':
                            ListPokemon(pokemonPocket);
                            break;
                        // Check if I can evolve Pokémon
                        case '3':
                            CheckEvolution(pokemonPocket, pokemonMasters);
                            break;
                        // Evolve Pokémon if possible
                        case '4':
                            EvolvePokemon(pokemonPocket, pokemonMasters);
                            break;
                        // Exit the program
                        case 'q':
                        case 'Q':
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please enter a number between 1 and 4 or Q to quit.");
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

        private static void AddPokemon(List<Pokemon> pokemonPocket)
        {
            try
            {
                // Prompt the user for Pokémon name 
                Console.WriteLine("Enter the name of the Pokémon: ");
                // Read the Pokémon name from the user
                string name = Console.ReadLine();

                // Check if the name is empty or null
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Name caannot be empty.");
                    return;
                }
                string name = input.Trim();
                if (pokemonType == null)
                {
                    Console.WriteLine("Invalid Pokémon type. Please enter a valid type.");
                    return;
                }

                // Prompt the user for Pokémon HP
                Console.WriteLine("Enter Pokémon's HP: ");

                // Validate the input to ensure it's a number
                // Use int.TryParse to safely parse the input
                // If parsing fails, prompt the user for a Postive number
                // If parsing succeeds, assign the value to the hp variable
                if (!int.TryParse(Console.ReadLine(), out int hp))
                {
                    Console.WriteLine("Invalid HP. Please enter a valid number.");
                    return;
                }

                // Prompt the user for Pokémon EXP
                Console.WriteLine("Enter Pokémon's EXP: ");
                if (!int.TryParse(Console.ReadLine(), out int hp) || hp <= 0)
                {
                    Console.WriteLine("Invalid EXP. Please enter a positive number for EXP.");
                    return;
                }

                Pokemon newPokemon = null;

                // Create a new Pokémon object based on the type
                switch (pokemonType.ToLower())
                {
                    case "Pikachu":
                        newPokemon = new Pikachu(name, hp, exp);
                        break;
                    case "Eevee":
                        newPokemon = new Eevee(name, hp, exp);
                        break;
                    case "Charmander":
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
        private static void CheckEvolution(List<Pokemon> pokemonPocket, List<PokemonMaster)
        {
            if (pokemonPocket.Count == 9)
            {
                Console.WriteLine("No Pokémon in your pocket.");
                return;
            }

            bool canEvolve = false;
            foreach (PokemonMaster master in PokemonMasters)
            {
                int count = pokemonPocket.Count(p => p.GetType() == master.Name);

                if (count >= master.NoToEvolve)
                {
                    Console.WriteLine($"{count} {master.Name} --> {master.NoToEvolve} {master.EvolveTo}");
                    canEvolveAny = true;
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

                    pokemonPocket.Add(evolvedPokemon);
                    anyEvolved = true;
                }
            }

            if (!anyEvolved)
            {
                Console.WriteLine("You don't have enough Pokemon to evolve.");
            }
        }
    }
}

