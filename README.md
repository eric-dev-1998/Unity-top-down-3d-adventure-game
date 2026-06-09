# Unity top down 3D adventure game (Prototype)

This is a **Top Down 3D** game prototype built in **Unity**.
The purpose of this project was test how far can I go working on this type of games.

My main focus is in the game mechanics, but I did work in some other aspects of it such as: Level & character designs, game UI, animations and visual effects.

These are the main mechanics I managed to develop:
## Player movement:
The player can walk around by either walking slowly or sprinting to go faster. Player sometimes can be stopped during certain events to maintain a certain amount of coherence in the game like when the pause menu opens or during an even sequence.

## Combat & spell casting system:
The player can cast magic attacks of each element: Fire, Water, Wind and Earth as well as pure or "Neutral" magic.
Enemies inherit a class called "**SpellReactor**", this is highly customizable as it provides hook methods to implement unique reactions to the different magic elements.
This is not limited to enemies as static objects can be spell reactive as well. I used the dummy as an example of it.

Enemies follows a data driven structure, this means, an enemy behavior can be defined within the inspector, properties such as health, power, wandering & attack behavior can be set to build unique enemies.

As for this prototype i made two enemies, one is hostile and the other is not.

## The event sequence system:
This is the more complex systems and it's the one that make this type of game make sense.
The event sequence system, as the name says, can read a sequence of different type of events to produce event driven dialogues.

This sequences are built in a node editor I made to make it easier.

<img width="1919" height="1033" alt="image" src="https://github.com/user-attachments/assets/869c274a-dfe2-46cb-87ca-33f29f384d72" />

Dialogue lines are called by their ID on a custom **TextLibrary** I made to support text localization.

## Quest & Inventory:
NPC can trigger quests that player can consult on the quest menu or by looking at the quest notification cards that shows up whenever a quest is triggered, updated or completed. These quests are separated in two groups: Active and Completed.

Some of these quests require the player to obtain specific objects, this is where the inventory system takes place. The inventory system is presented as a list of object the player possess on the inventory menu.

The Inventory and Quest system are connected and can listen to each other.
- The Inventory system will notify the quest system whenever an item is being added so the quest manager will look up for any active quest and see if this new item matches any quest objectives.
- The quest system will ask the inventory manager when a quest is triggered to see if any of it's item related objectives is completed already. 
