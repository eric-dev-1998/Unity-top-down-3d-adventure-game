# Game design notes:

## 1. Game collectables:
### 1. Magic stones:
    These refill the health meter and serves for scoring as well, like the coins in super mario 64.
### 2. Magic crystals: 
    Used to refill the magic meter.
### 3. Magic sprites:
    Creatures to be rescued, 5 per level will be good. Collecting them will grant a magic orb.
### 3. Magic orbs:
    These are the main progress collectables, like the stars in mario 64.

## Add magic spell data to game UI:
### Where should this data be presented in the game?
### 1. The HUD:
The hud should display a control that shows what magic element is selected.

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

## Add level specific information:
### Information to display:
1. Collected magic stones.
2. Collected magic sprites.
3. Collected magic orbs.

Just numbers to keep de layout clean and simple.

### Where should this data be displayed at:
1. On the pause menu when the player is inside a level.
2. Before entering a level, just by standing near the entrance this information should appear on screen.