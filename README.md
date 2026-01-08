# Unity top down 3D adventure game (Prototype, Work in progress)

This repository contains a **personal Unity project** developed as part of my growth as a game developer and software engineer.  
The goal of this project is to demonstrate **game systems design, clean code structure, and Unity development practices**, rather than to ship a commercial product.

---

## 🎮 Project Overview

This is a small adventure-style game prototype featuring:
- Player movement and interaction
- NPC behavior and basic AI
- Modular game systems built with scalability in mind
- ScriptableObjects for data-driven design

The project was built to explore how different gameplay systems interact in a real Unity project.

---

## 🛠️ Technologies & Tools

- **Unity** (version 6000.3.3f1)
- **C#**
- Unity Input System
- ScriptableObjects
- Git & GitHub for version control

---

## 🧠 Key Systems Implemented

- **Player Controller**
  - Movement, interaction, and state handling
- **Dialogue system**
  - Mid-level zelda-like dialogues system.
- **Inventory system**
  - Basic list style inventory system.
- **Quest system**
  - Mid-level quest system.
- **NPC System**
  - Basic AI logic and interaction flow
- **Game Data Architecture**
  - Use of ScriptableObjects for configurable data
- **Prefab-Based Design**
  - Reusable and modular components

Each system is designed to be readable, maintainable, and easy to extend.

---

## 📁 Project Structure

```text
Assets/
 ├── Art/              # Game artistic elementes: audio, textures, models, animations, etc.
 ├── Editor/           # Custom unity editor tools.
 ├── Prefabs/          # Reusable game objects
 ├── Resources/        # In-Game readable assets: quest data, dialogue data, etc.
 ├── Scenes/           # Game scenes.
 ├── ScriptableObjects/# Data-driven configurations
 ├── Scripts/          # Core gameplay logic and systems
 └── Shaders/          # Custom and Unity shaders
 └── Third party/      # Third party assets.
