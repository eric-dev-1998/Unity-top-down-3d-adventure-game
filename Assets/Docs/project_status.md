Systems the project already has:

1. Player
2. NPC
3. Dialogue system
4. Event system
5. Inventory system
6. UI system
7. Test levels
8. Camera system

Player:
1. What works?
	Player can move arround and rotates towards direction of movement.
	Player can detect trigger collisions and can be detected as well.
	Player can play some basic motion animations: Idle, Walk, Run and Pickup
	Player can cast wind magic and its collision can be detected as well.
2. What is half done?
	a. The player mid-air motion animations still need to be implemented.
	b. The player input read should detect if a gamepad is on use and apply needed settings
	   so player controls are comfortable with both gamepad and keyboar-mouse modes.
	c. Spell casting system.
3. What is broken?
	a. Player run animation looks bad because player's head is always looking down.
4. What needs refactoring?
	a. Probably how a sequence of events may affect player behaviour.
5. What is missing?
	a. Basic interaction animations like, giving to a npc, taking from an npc, speak, etc.

NPC:
1. What works?
	a. A npc can look at player when its near.
	b. Player can interact with an NPC to triger an event sequence.
2. What is half done?
	a. NPC animator controller.
	a. Path following system.
3. What is broken?
	a. Nothing.
4. What needs refactoring?
	a. The path system could probably be affected by the event system.
5. What is missing?
	a. Nothing.

Dialogue system:
1. What works?
	a. The dialogue system core functions like: display dialogue, hide dialogue and print dialogue text are
	   fully functional.
2. What is half done?
    b. Interaction with the new event system.
3. What is broken?
	a. Nothing.
4. What needs refactoring?
	a. Nothing.
5. What is missing?
	a. Nothing.

Event system:
1. What works?
	a. The event graph builder UI has been designed.
2. What is half done?
	a. The graph builder on its entirety.
	b. The event manager logic.
3. What is broken?
	a. Graph asset save functionallity.
	b. Graph asset load functionallity.
	c. Graph project creation functionallity.
4. What needs refactoring?
	a. Nothing.
5. What is missing?
	Graph to event sequence compilation.

Inventory system:
1. What works?
	a. The inventory UI was designed but its not definitive.
	b. Items can be added.
	c. Items can be thrown away.
	d. Items can be consumed.
2. What is half done?
	a. Nothing.
3. What is broken?
	a. There is a bug that prevents items icons from beign loaded and rendered properly.
4. What needs refactoring?
	a. Nothing.
5. What is missing?
	a. A definitive UI design, but this involves designing the full game UI as well.

UI System:
1. What works?
	a. Game HUD which is not definitive.
	b. Dialogue UI, which is not definitive either.
	c. In game menu UI, which is not definitive.
	d. Inventory UI, which is not definitive.
2. What is half done?
	a. Item obtaining UI.
3. What is broken?
	a. There is a bug on the inventory system that prevents items icons from beign loaded
	   and rendered properly.
4. What needs refactoring?
	a. Nothing.
5. What is missing?
	a. A shop like UI if a shop is finally implemented.

Test levels:
1. What works?
	a. The water shader looks good.
	b. Desert sand dunes can be triggered to disappear when casting wind on them.
2. What is half done?
	a. The desert terrain model.
	b. The desert nature models.
	c. The desert environment models.
	d. Test npc.
	e. Test npc dialogues.
3. What is broken?
	Nothing.
4. What needs refactoring?
	Nothing.
5. What is mising?
	a. Test level 2 (Earth)
	b. Test level 3 (Ice)
	c. Test level 4 (Fire)

Item pickup:
1. What works?
	Items can be picked up when player is near.
2. What is half done?
	The dialogue event system needs to be finished to implement pickup dialogues and other events
	when required based on the game story.
1. What is broken?
	Nothing.
4. What needs refactoring?
	Nothing.
5. What is missing?
	Nothing.

Interactables:
1. What works?
	Interactables like a door can be triggered when player is near enough.
2. What is half done?
	An interactable object may require the use of dialogue events when needed based on game's story.
3. What is broken?
	Nothing.
4. What needs refactoring?
	Nothing.
5. What is missing?
	Nothing.

Camera system:
1. What works?
	The camera smoothly follow its target which can be changed in runtime to simulate basic motions
	for cutscenes as an example.
2. What is half done?
	Nothing.
3. What is broken?
	Nothing.
4. What is missing?
	Nothing.