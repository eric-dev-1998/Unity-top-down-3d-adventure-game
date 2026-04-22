# Game design notes:

## 1. Game collectables:
### 1. Life stones:
    These refill the health meter and serves for scoring as well, like the coins in super mario 64.
### 2. Magic crystals: 
    Used to refill the magic meter.
### 3. Lost sprits:
    Creatures to be rescued, 5 per level will be good. Collecting them will grant a power orb.
### 3. Power orbs:
    These are the main progress collectables, like the stars in mario 64.

### To do:
1. [Done] Design life stone.
2. [Done] Design magic crystal.
3. [Done] Design lost spirit.
4. [Done] Design power orbs.
5. Define where these collectables counters will be stored.
6. Add the counters on the defined class/script.
7. Define where these counters are going to be displayed in the HUD.
8. Define where these counters are going to be displayed in the pause menu.
9. Create UI sprites for these counters.
10. Implement counters to the HUD.
11. Implement these counters to the pause menu.
12. Create a monobehavior class for all collectibles with a type property (Life_Stone, Magic_Crystal, Lost_Spirit and Power_Orb).


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