# Game design notes:

## 1. [Done] Game collectables:
### 1. Life stones:
    These refill the health meter and serves for scoring as well, like the coins in super mario 64.
### 2. Magic crystals: 
    Serves as a magic bar filler and are needed to unlock certain areas.
### 3. Lost sprits:
    Creatures to be rescued, 5 per level will be good. Collecting them will grant a power orb.
### 3. Power orbs:
    These are the main progress collectables, like the stars in mario 64.

### How are these collectables be presented in the game nenu:
These collectables records will be displayed on the inventory in the lower half, right where the items name and description are displayed.
To do that, the items name and description will be displayed in a dialogue box instead.

### To do:
1. [Done] Design life stone.
2. [Done] Design magic crystal.
3. [Done] Design lost spirit.
4. [Done] Design power orbs.
5. [Done] Define where these collectables counters will be stored.
6. [Done] Add the counters on the defined class/script.
7. [Done] Define where these counters are going to be displayed in the HUD.
8. [Done] Define where these counters are going to be displayed in the pause menu.
9. [Done] Create UI sprites for these counters.
10. [Done] Implement counters to the HUD.
11. [Done] Make the inventory system display the items info on a dialogue box.
12. [Done] Implement these counters to the pause menu (Were placed on the inventory menu instead, a input to open the inventory menu directly would be great.).
13. [Done] Create a monobehavior class for all collectibles with a type property (Life_Stone, Magic_Crystal, Lost_Spirit and Power_Orb).


## [Done] Add magic spell data to game UI:
### Where should this data be presented in the game?
### 1. The HUD:
The hud should display a control that shows what magic element is selected.
The player should be able to select a magic element by scrolling with the mouse.

### 2. The inventory:
The inventory should include a section to show magic power related data:
1. Unlocked elements.
2. Mana cost per element.

### What does this implies?
1. No mayor changes should be needed, elemental orbs can be presented as items on the actual layout. Information such as a description and mana cost can be checked by selecting them on the list.

### What to do?
1. [Done] Finish the orb pedestal to make it give an orb to the player.
2. [Done]  Make item data for each orb.
3. [Done] Make the player unable to use powers before unlocking them.
4. [Done] Make spells consume a fixed ammount of magic.

## Add level specific information:
### Information to display:
1. Collected magic stones.
2. Collected magic sprites.
3. Collected magic orbs.

Just numbers to keep de layout clean and simple.

### Where should this data be displayed at:
1. On the pause menu when the player is inside a level.
2. Before entering a level, just by standing near the entrance this information should appear on screen.

### What to do?
1. [Done] Make spells comsume a certain ammount of magic depending on each spell.
2. Define magic stones behavior.
3. Design magic stones.
3. Implement magic stones.

## Enemy AI & behavior:
This game is now heavily inspired on games such as Super Mario 64 & Banjo Kazooie. The expected enemy AI & behavior has to be similar.

### The objective:
Create an abstract class to make many different enemies in which an AI & behavior can be set or modified beforehand inside the unity editor. Other settings such as health should also be able to be set or modified inside the unity editor.
Each sub class can have their own settings/properties based on the desired behavior.

### Expectations for the Demo:
Develop the base enemy class & create 3 different enemy types with simple behaviors:
1. Dummy
2. Melee
3. Ranged

### Expectations for the final game:
There should be more than one enemy for each level environment, right now the levels are:
1. Witch Hut.
2. Mountains level.
3. Shore level.
4. Forest level.

### Test enemies:
    [Not included in the demo] Spike: The spiky boulder.
        AI behavior: Neutral
        Attack behavior: Melee

    Fungy: The evil fongus.
        AI behavior: Hostile
        Attack behavior: Ranged

## [Done] Magic stones:
Previously called "Magic orbs" are now called magic stones.

### [Done] Magic core: 
A gloomy and shiny crystal surrounded by magic particles.
### [Done] Fire stone: 
Shaped as a flame, covered in fire.
### [Done] Water stone: 
Shaped as a water drop, covered in blue water particles that orbit around it.
### [Done] Wind stone: 
Transparent sphere with a rotating spiral inside covered by wind particles.
### [Done] Earth stone: 
Shiny stone surrounded by floating pebbles.
