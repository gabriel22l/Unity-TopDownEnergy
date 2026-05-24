# Unity-TopDownEnergy

A top-down prototype developed in Unity 6000.3.8f1 as a 4-week university project.
Features include an inventory system, energy system (solar, battery, consumers), day/night cycle, and base building.

Players collect resources, construct renewable energy structures, and maintain energy balance across their base.

## Table of Contents

- [Project Overview](#project-overview)
- [Current State](#current-state)
- [Features](#features)
- [Systems](#systems)
- [Screenshots](#screenshots)
- [Future Improvements](#future-improvements)
- [What I Learned](#what-i-learned)
- [Technology Stack](#technology-stack)
- [Credits](#credits)

## Project Overview

**TopDownEnergy** is a 4-week university project designed to explore sustainable energy management through interactive gameplay. Players gather resources, construct renewable energy structures, and plan their builds to maintain stability and keep the base powered through the night.

## Current State

The project is currently a gameplay prototype focused on core systems rather than final art or content.

## Features

- **Energy Management System**: Dynamically track energy production, consumption, and storage
- **Resource Inventory**: Collect, manage, and spend resources for construction. Supports drag-and-drop item management
- **Base Building**: Construct structures on designated slots with validation of resources and energy costs
- **Renewable Energy Structures**: Place energy producers (solar panels, wind turbines) and storage facilities
- **Energy Consumers**: Structures that consume energy while active
- **Player Interaction System**: Interact with world objects, resource gathering and access terminals
- **UI Management**: Inventory and energy status displays

## Systems

### Core Systems

**Interaction System** (`Player/`)
- Trigger-based detection of nearby interactables via `IInteractable`
- Automatically targets the closest interactable in range
- Visual feedback on the current target via sprite color shift
- Exposes player systems to interactable objects through a shared `InteractionContext`

**Energy Controller** (`BaseManagement/`)
- Energy production, consumption, and storage
- Interval-based energy simulation (1-second intervals by default)
- Registers and manages all energy producers, consumers, and storage structures
- Prevents energy overflow and handles insufficient power scenarios

**Base Manager** (`BaseManagement/`)
- Manages base slots (building positions) and structure placement
- Validates building requirements (resources + energy costs)
- Controls structure uniqueness constraints
- Binds to the player inventory on terminal access for building validation
- Emits slot change events on successful builds

**Inventory System** (`InventorySystem/`)
- Slot-based inventory with drag-and-drop UI interaction
- Item data managed via ScriptableObjects
- Tracks resource quantities and validates availability for building

**Player Controller** (`Player/`)
- Character movement with input normalization
- Facing direction tracking for animations
- Player-mounted light system for nighttime visibility

**UI System** (`UI/`)
- Inventory display synchronized with data model
- Energy status visualization
- Menu management

### Design Patterns

Leverages event-driven communication to reduce direct dependencies between systems.

- **Observer Pattern**: Structural states, inventory changes, and energy updates emit events, which UI elements subscribe to, reducing direct dependencies from logic to presentation.
- **Data-Driven Design (ScriptableObjects)**: Item properties, structural configurations, and construction costs are defined as assets, allowing for data changes without modifying code.
- **Behavioral Composition**: Separation of concerns (Movement, Input, Animation, etc.), behavior is composed from focused, single-responsibility components.

## Screenshots
### Inventory
<img src="Images/InvGif2.gif" width="500">

### Gameplay
<img src="Images/Screenshot_1.png" width="500"> <img src="Images/ScreenShot_4.png" width="500">

### Building Interface
<img src="Images/Screenshot_3.png" width="500">

### Energy Interface
<img src="Images/Screenshot_5.png" width="500">

## Future Improvements

- **Procedural Generation**: Randomize layouts for replayability
- **Multiple Building Types**: More diverse energy consumers and producers
- **Expanded Inventory**: Equipment, consumables
- **Save/Load System**: Persistent gameplay state
- **Environmental Feedback**: Weather effects impacting solar/wind production
- **Tutorial System**: Guided introduction for new players

## What I Learned

- **Energy System Design**: Creating balanced, tick-based resource simulation mechanics
- **UI/Data Binding**: Synchronizing UI with gameplay data through events and state-driven updates
- **Inventory Management**: Designing flexible slot-based systems with drag-and-drop interactions
- **Game Architecture**: Building scalable, maintainable systems with clear separation of concerns
- **ScriptableObject Pipeline**: Data-driven design for flexible content creation
- **Input Handling**: Modern Unity Input System usage for responsive controls
- **Game Feel**: How visual feedback (lighting, animations) enhances player experience
- **Time Management**: Delivering a functional project within a 4-week university timeframe

---

## Technology Stack

- Unity 6 (6000.3.8f1)
- C#
- Universal Render Pipeline (URP)
- Unity Input System
- Cinemachine
- TextMeshPro

---

## Credits

### Programming & Systems Design
- Gabriel Guzmán

### Pixel Art & Sprites
- Cynthia Pérez

### Third-Party Assets
- pixel-boy (itch.io, CC0)

---
**Development Time**: 4 weeks