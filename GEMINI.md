# GEMINI.md - Project Context

## Project Overview
**RTS-RI-Project-1** is a 2D Real-Time Strategy (RTS) game with roguelike progression elements, developed using **Unity 6 (6000.1.7f1)** and the **Universal Render Pipeline (URP)**.

### Core Mechanics
- **Combat**: Real-time battle system where players control a squad of units against waves of enemies. Units use the **A* Pathfinding Project** for movement and have stats like health, damage, and fire rate.
- **Roguelike Progression**: A node-based map system where players choose missions of varying difficulty (Easy, Medium, Hard, Final).
- **Meta-game**: Features a Shop for acquiring characters, a Squad Manager for team composition, and an Economy system (Gold).
- **Persistence**: Save system for map state, squad, and inventory.

### Key Technologies
- **Unity 6**: Modern Unity engine features.
- **Universal Render Pipeline (URP)**: 2D lighting and rendering.
- **A* Pathfinding Project**: Advanced AI navigation.
- **Unity Input System**: New event-driven input handling.
- **TextMeshPro**: High-quality UI text rendering.

## Project Structure
- `Assets/Script/Battle/`: Core combat logic (Units, Spawners, Game Controller).
- `Assets/Script/Main Menu/`: Meta-game systems (Map, Shop, Squad, Inventory).
- `Assets/Prefabs/`: Reusable game objects for units, UI, and environmental elements.
- `Assets/Scenes/`: Game levels, including the Main Menu and various Battle scenes.
- `ProjectSettings/`: Unity project configuration.

## Building and Running
### Requirements
- Unity Hub and Unity Editor version **6000.1.7f1**.

### Execution
1. Open the project in Unity Editor.
2. Open `Assets/Scenes/Main Menu.unity` (or the initial scene).
3. Press the **Play** button to run in the editor.

### Building
1. Go to `File > Build Settings`.
2. Ensure all necessary scenes (Menu and Battle variations) are in the "Scenes In Build" list.
3. Select the target platform and click **Build**.

## Development Conventions
- **Scripting**: Use C# with standard Unity conventions.
- **Naming**: PascalCase for Classes and Public members; camelCase for private fields.
- **Inspector**: Use `[SerializeField]` to expose private variables to the Unity Inspector.
- **Dependencies**: Use `[RequireComponent]` for mandatory component dependencies.
- **Team Identification**: Units and bullets are identified via Tags (`PlayerUnit`, `EnemyUnit`, `Bullet`) and `teamID` in `UnitStats`.
- **Roguelike State**: Managed via `SquadTransferData` for data passing between scenes and `SaveManager` for persistent storage.

## Key Files
- `Assets/Script/Battle/GameController.cs`: Manages unit selection and movement commands.
- `Assets/Script/Battle/WaveSpawner.cs`: Handles enemy spawning logic.
- `Assets/Script/Battle/UnitStats.cs`: Core data and health management for units.
- `Assets/Script/Main Menu/Map Manager/MapManager.cs`: Controls the roguelike mission progression.
- `Assets/Script/Main Menu/Squad Manager/SquadManager.cs`: Manages the player's active team.
