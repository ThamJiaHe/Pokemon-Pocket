# Pokémon Pocket Application

A comprehensive Pokémon management system with battle arena, training center, evolution mechanics, and data persistence.

## Features

### Core Features
- **Add Pokémon**: Create Pikachu, Eevee, or Charmander with custom name, HP, and EXP
- **List Pokémon**: View all Pokémon sorted by experience points in descending order
- **Evolution System**: Check eligibility and evolve Pokémon when requirements are met
- **Battle Arena**: Battle your Pokémon against other players or AI opponents
- **Training Center**: Improve your Pokémon's stats through various training methods
- **Data Persistence**: Save and load your progress using SQLite database or CSV files

### Battle System
- **PvP and PvAI Modes**: Choose between battling another player or an AI opponent
- **Turn-Based Combat**: Select attacks or defensive moves each turn
- **Type Effectiveness**: Damage multipliers based on Pokémon type matchups
- **Critical Hits**: 10% chance for double damage
- **Defensive Stance**: Reduce incoming damage by 50%

### Training System
- **HP Training**: Direct HP improvements
- **Battle Training**: EXP gains for potential evolution
- **Elite Four Challenge**: Risk/reward system with variable outcomes

### Evolution Mechanics
- **Pikachu → Raichu**: Requires 2 Pikachu
- **Eevee → Flareon**: Requires 3 Eevee
- **Charmander → Charmeleon**: Requires 1 Charmander
- **Visual Animations**: Engaging evolution sequence

### User Experience
- **Color-Coded Messages**: Green for success, red for errors, yellow for draws
- **Battle Animations**: Visual cues for critical hits and move effectiveness
- **Intuitive Menu System**: Clear navigation with validation

## Technical Implementation

- **Object-Oriented Design**: Inheritance hierarchy for Pokémon types
- **Polymorphism**: Different damage calculations based on Pokémon type
- **SQLite Integration**: Persistent storage with proper entity mapping
- **File-Based Import/Export**: CSV file support for data portability
- **Exception Handling**: Robust error management throughout the application

## Installation

1. Clone this repository
2. Ensure you have .NET SDK installed
3. Navigate to the project directory
4. Run `dotnet build` to compile the application
5. Run `dotnet run` to start the application

## Usage

The application presents a menu-driven interface:
1. Add Pokémon to your pocket
2. List Pokémon in your pocket
3. Check if Pokémon can evolve
4. Evolve Pokémon
5. Battle Arena
6. Training Center
7. Save & Load

## Credits

Developed by THAM JIA HE (244548T) - 2025

Inspired by the Pokémon games and created as part of an advanced programming assignment.

## License

Copyright © 2025 THAM JIA HE. All rights reserved.

## Acknowledgments

Parts of this project were developed with assistance from GitHub Copilot, an AI programming assistant. The AI was used as a programming assistant only, with all generated code being reviewed, understood, and modified before inclusion.