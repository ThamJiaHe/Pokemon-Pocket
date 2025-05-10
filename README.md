# Pokémon Pocket System

A command-line application for managing a Pokémon database, allowing you to add, search, update, and delete Pokémon information.

## Features

- Add new Pokémon with details (ID, name, type, stats)
- Search for Pokémon by name or ID
- Search for Pokémon by type
- View all Pokémon in a formatted table
- Update Pokémon information
- Delete Pokémon from the database

## Installation

1. Ensure you have Python 3.6+ installed
2. Install the required packages:
   ```
   pip install -r requirements.txt
   ```

## Usage

Run the application:
```
python pokemon_pocket.py
```

Follow the on-screen menu to interact with the system:
1. Add a new Pokémon
2. Search Pokémon by name/ID
3. Search Pokémon by type
4. View all Pokémon
5. Update Pokémon information
6. Delete Pokémon
7. Exit


```
Program Features:
Base Pokémon Class and Subclasses:

Implemented abstract Pokemon class with required attributes
Created subclasses for Pikachu, Eevee, and Charmander
Added evolved forms (Raichu, Flareon, Charmeleon)
Implemented specific damage calculation for each Pokémon type
Menu System:

Option 1: Add Pokémon to Pocket
Option 2: List Pokémon in Pocket (sorted by Experience)
Option 3: Check Evolution availability
Option 4: Evolve Pokémon
Option 5: Pokémon Battle (special feature)
Q: Exit Program
Input Validation:

Implemented comprehensive validation for all user inputs
Catch exceptions to prevent program crashes
Evolution System:

Check which Pokémon can be evolved based on requirements in PokemonMaster list
Evolve multiple Pokémon at once
Set evolved Pokémon stats (HP=100, Exp=0)
Special Battle Feature:

Players can select two Pokémon to battle
Calculates damage according to each Pokémon's damage multiplier
Rewards winner with extra experience points
Restores HP after battles
Shows detailed battle progress