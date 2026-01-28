### 12/9/2025:
Project status report was made and saved to 'project_status.md'.
Project vision was updated and saved to 'vision.md'.
Features report was made, but there are empty sections: Items, Characters, Art assets. 
These features requires the story to be designed.
Devlog file has been created.

Next: Prepare a lightweight kanban board.

### 12/9/2025 17:50pm:
A github repository has been created and an empty project lightweight kanban board too.

Next: The progress board needs to be filled with tasks.

### 12/10/2025 12:31pm:
The first task has been added: "Re organize the project folder structure".

Next: Finish the first task and Review project status and features to establish first official tasks.

### 12/10/2025 12:42pm:
A camera system reference was added to the project status.

Next: Finish the first task and review project status and features to establish the first official tasks.

### 12/10/2025 13:20pm:
The project folder structure has been re organized properly.

Next: Establish the first official tasks.

### 12/10/2025 13:51pm:
The first taks have been placed on the progress board.

Next: Work on the first task on the board.

### 12/10/2025 18:35pm:
A problem with new nodes added right before deserializing a graph node in the dialogue graph editor have been fixed
and now new nodes added right before deserializing an graph asset are placer properly right where created.

Next: Fix the bug that produce nodes duplication when a graph asset is deserialized and new nodes are added.

### 12/11/2025 12:59pm:
A problem that make SaveProject() in the Dialogue graph editor logic that threw a null exception has been fixed.
Now projects can be saved properly.

Next: Node propertie are not being loaded into the nodes fields, it has to be fixed.

### 12/11/2025 14:00pm:
A lot of things have to be refactored in the dialogue graph view code, turns out that no "local_nodes" list is required
as every node will be stored in a graph asset in runtime.

Next: Re desing DialogGraphView workflow.

### 12/11/2025 19:46pm:
I realized that the dialogue graph editor code can be optimized by removing some things as described in the github task.

Next: Take the time to reconsider this changes before taking action.

### 12/12/2025 18:23pm
Improvements were made in the dialogue graph editor code, some code has been removed. Graph asset serialization and deserilization
are working now.

Next: Implement node connections (Edges) in these processes.

### 14/12/2025 17:40pm:
I added the code to implement node connections on serialization and deserialization. Connections seems to be properly serialized as
they're visible within the graph asset content at the inspector.

### 14/12/2025 18:09pm:
Connection cannot be created, but for some reason, a graph asset of two nodes throws a total count of 4 when calling 'runtime_nodes.Count. That may have something to do with it.

### 12/15/2025 9:11am:
The bug that made nodes get duplicated was fixed, but the problem that prevents a connection
from beign reated still persist.

### 12/15/2025 9:21am:
The bug that prevented node connections from beign created is fixed.
Graph serialization and deserialization seems to be working now.

### 12/15/2025 9:47am:
UI buttons have been made responsive, this means, save, close and build buttons will now disable if no graph asset is loaded.

### 12/15/2025 11:38am:
The preliminar node types catalogue has been designed.

Next: Implement graph build logic to test with a single note type before adding more node types.

### 12/15/2025 14:19pm:
Dialogue graph build logic implementation is under way, a event sequence can be built.
Only a single node type exsist right now.

Next: Implement single event runtime logic and event manager runtime logic as well.

### 12/15/2025 16:22pm:
Event manager and single event logic have been written, but for some reason, wither the event manager or in the build logic the created events are the base type (Event) not their target type which in this test scenario should be SingleLine.

Next: Figure out why the events inside the event sequence are all the same type (the base type: "Event") and test.

### 12/16/2025 11:58pm:
Event sequence can now be built.

Next: Complete the event manager logic.

### 12/16/2025 14:08pm:
Event sequence can now be built.
Manager can handle event to display a dialogue properly.

Next: Fix player movement and implement event trigger for different scenarios.

### 12/17/2025 11:33am:
The dialogue graph editor can now open graphs by double cliking on them or by drag and dropping
them into the editor.

The possible trigger scenarios were designed and are not final:
1. Npc interaction
2. Pickup trigger
3. Area trigger.

Next: Start implementing node and event types.

### 12/17/2025 16:57pm:
A new node type has been implemented but event logic has not been implemented yet.

Next: Implement the new event logic.

### 17/12/2025 18:39pm:
Multiline node has been implemented.

But local variables need to be properly declared to be used in every switch case to avoid declaring a lot of unnecessary variables.

Next: Find a better way to declare variables in NodeFactory to avoid the use of lots of variables.

### 18/12/2025 11:54am:
Question node has been implemented.

But there is a bug that causes the dialogue system to behave weird.

Next: Fix that behaviour bug.

### 18/12/2025 13:22pm:
Question node has been completely implemented and the dialogue bug has been fixed.

Next: Look for ways to optimize the following codes:
1. Node & subNodes clases
2. Node factory

### 19/12/2025 9:15am:
Node & sub nodes were optimized but just a bit to avoid wasting more time, further optimization will
take on later stage of the project.

Node factory remains with no changes.

Next:
The first text level with desert theme will have a single quest: Find the missing object under the sand.
This will need the following features:
1. Basic quest system
2. Item pickup event
3. NPC interaction event
4. Door interaction event
5. Sand dune interaction

### 19/12/2025 15:54pm:
A new game menu UI has been designed.
New game menu UI implementation is on the way.
New item structure implementation on inventory system is on the way.

Next: Finish it.

### 20/12/2025 20:49pm:
Item nodes and events are implemented an are fully functional.
Items 3d models are shown as a preview when the item get or buy events take place.

### 22/12/2025 11:39am:
Dynamic event sequence creating has been implemented for item pickup trigger.

Next: Imlpement new UI design.

### 23/12/2025 16:33pm:
New UI design has been applied to the in-game menu.
Dialogue UI will need to be re designed, but that's for the future.

Next: Implement Quest System.

### 25/12/2025 11:50am:
The Quest System development has started.

### 25/12/2025 14:48pm:
The quest system basic functionality is now working, so quest can now be activated and a message will pop up in game hud and will be displayed in the quest menu.

Next:
1. Make completed quests move to the 'completed' list.
2. Implement nodes in graph view editor to trigger quests through events.
3. Stablish connections between event system and quest system to trigger quest updates.
4. Quest track system.

### 29/12/2025 11:50am:
A notification shows up when a quest item objective status is updated.

Next: Current dialogue ui system is kinda broken so its going to be updated to a ui document.

### 28/12/2025 16:34pm:
Dialgue UI has been updated to a UIDocument, so dialogues work again.
Quest nodes need to be implemented to advance.

Next: Implement quest nodes. It could probably use a single node for tigger and completion logic and another one to check quest objectives status like "are this quest objectives marked as completed?".

### 30/12/2025 11:27pm:
Inventory items are now displayed in the inventory menu, display and description are not displayed on selection yet.

Next: Implement quest nodes.

### 30/12/2025 16:55pm:
Quest node for activate and completion have be implemented (partialy).

Next:
Make the state dropdown display choices.
Make the quest dropdown display a default choice.
Make the state dropdown display a default choice.
Implement event functionallity.
Implement quest state check node.
Make sure the game object event node works properly (I blelieved its used to display items on dialogues so far).

### 31/12/2025 10:22am:
Quest "set state" node has been implemented: Quest can now be triggered and completed via event nodes.

Next:
Implement quest state check node.
Make sure the game object event node works properly.

### 21/12/2025 13:43pm:
Quest state node implementation has started:

Next:
Make node display only the output node.
Implement event functionallity.
Make sure the game event node works properly.

### 01/01/2026 15:15pm:
Quest get status node is properly displayed.

Next:
Implement event functionallity.
Make sure the game event node works properly.

### 01/01/2026 16:04pm:
Quest event functinallity has been implemeted.

Next:
Make sure the game event node works properly.

### 01/01/2026 16:24pm:
Every node and its event functinallity has been implemented and its currently working.

### 01/01/2026 17:22pm:
Quest system is working and is connected with both event and inventory systems. Which means:
1. Quest system responds to changes on player's inventory.
2. Quest system responds to object interactions.
3. Quest system responds to player entering areas.

Next:
The use of a quest track system need to be decided before moving on.
The new NPC interaction system has to be desiged.

### 01/02/2026 11:52am:
Quest track system was finally implemented.

Next:
Re-design and implement a new NPC interaction algorithm so NPC's have more naturar
dialogues. More details on GitHub.

###
A string collection asset should be created, it will contain all the game related texts
such as dialogues, info texts, menu text and any text that is used in the final game.
This will allow to have texts in different languages.

### 01/05/2025 14:55pm:
Text library system has been implemented, but its not finished. More details in github.

Next: Design the new NPC interaction system and implement it.

### 01/06/2026 16:05pm:
New Npc interaction system development has started.

Next: 
Main logic has been implemented but has not been tested, to do so
a way to convert the npc dialogues to a sequence of events have to be implemenetd.

### 01/07/2026 11:59am:
Text library dialogue text list structure was updated.
Updates are beign made to the new npc interaction system.

Next: Verify the "CheckOwnQuest" function.

### 01/07/2026 14:47pm:
A prototype of the new npc interaction system has been implemented.

Next:
Apply the text library functionality to the dialogue graph editor.
Test the npc interaction system.

### 01/07/2026 15:43pm:
The first dialogue graph node has been updated to the new text library system.

Next: Update event functionality and apply to the rest of nodes.

### 01/09/2026 14:31pm:
The new Npc interaction systems works on a basic level. This means, a npc can check its own quests for availability and trigger if conditions match, and give a follow up dialogue while this quests are active.

Next:
1. Seems like its always  the same standard dialogue beign selected instead of beign random picked when calling for a standard dialogue.
2. Try more tests with main story quests instead of npc owned quests.

Pending:
1. Make the inventory system display a preview and item information when selected and clear item data from screen every time the inventory menu is opened.
2. Apply text library compatibility to pending nodes and events.

### 01/09/2026 9:56am:
TO-DO Summary:
1. [Quest system]: Add a 'from' field to preview menu.
2. [Quest system]: Update objectives display to toggles in the preview menu.
3. [Animation]: Create a task for human character action and emotion animations.
4. [Dialogue graph]: Design a string format that the graph view editor can understand it to properly place the resulting event on the precise folder.
5. [Dialogue graph]: Update question and multiline nodes to be comptible with the new text library system.
6. [Assets]: Stop using a "Scriptable objects" folder and migrate to Resources folder only.

### 01/13/2026 9:33am:
Dialogue graph event outputs are saved on separate folders using a save command.

Next: Update question and multiline nodes and their events.

### 01/13/2026 10:59am:
Question and multiline nodes have been updated to be compatible with the new text library system.

Next:
1. [Quest system]: Add a 'from' field to preview menu.
2. [Quest system]: Update objectives display to toggles in the preview menu.

### 01/13/2026 14:49pm:
Added a 'from' field to the quest preview menu.
Updated objectives display to styled toggles.

Next:
1. Do more test for the npc interaction system.
2. Test main quest validation on the npc interaction system.

### 01/14/2026 16:50pm:
A bug in the node graph editor that prevented nodes with multiple ports to connect properly has been fixed.

Npc dialogue is improving, and events are responding very well.

Next:
1. Somehow, 'lastMainQuestID' is beign set before event validating it in the code when checking for available main quests.

### 01/16/2026 11:33am:
The npc dialogue main quest check code was polished a little, but the same problem occours: some how lastMainQuestId is beign set before the manager check the available quest id.

### 01/16/2026 11:45am:
The previous problem was fixed by making the npc dialogue manager stop when a dialogue
case started. This also helps optimiztion by avoding unnecesary quest validation processes.

Right now, the npc dialogue system seems to work, it is not final though.

Next:
1. Door interaction.
2. Sand dune interaction.

### 01/16/2026
First quest for the deser demo level has been desingned and the dialogue events have been build, but it needs the following:
1. The inventory system will understand that an id value of "any" means inventory.count > 0.

### 01/18/2026 9:38am:
Created a new animation node for the dialogue graph editor.

Next:
1. Create a camera node for camera motion in game.
2. Implement this 2 nodes in desert level events.

### 01/19/2026 11:04am:
The first demo level (Desert) quest has been updated, camera can now focus different objects diring a event sequence and object animations can be played.

Next:
1. [] Door opens when interacted but doesnt do anything before that.
2. [Done] Polish player motion: Apply gravity to player.
3. [] Polish player motion animations.
4. [] Add human interaction animations: Talk.
5. [] Add a highlight icon for interactable objects.
6. [] Add water efects: walking on shallow water, walking on deep water, water ambience sfx.
7. [] Add walking effects: material based footsteps, material based step particles.

### 01/20/2026 15:06pm:
Started working with player audio, specifically footsteps audio.

To do:
1. [Done] Re organize main player script.
2. [] Create a physics specific class for entities.
3. [] Modify "Footsteps.cs"  apply these changes.

### 01/21/2026 13:31pm:
Re organized player code.

PlayerCore.cs
1. Input
2. Entity.cs
    1. Movement
    2. Collision
3. PlayerAudio.cs

### 01/22/2026 14:16pm:
The first demo level exit door is now working.

### 01/23/2026 11:45am:
A bug with player head rotation has been fixed.

### 01/23/2026 14:23pm:
Human animation have been polished a little and they work for now.

Next: 
Add animation/motion for: walk on deep water

### 01/23/2026 15:01pm:
Added motion animation for walking on deep water.

Next:
Add water motion sfx.

### 01/24/2026 10:58pm:
Updated footsteps logic.

Next: Polish animations a little more.

### 01/24/2026 12:12pm:
Improved player movement, animation and footstep sfx sync.

Next:
Imlpement water ripple and splash effects.

### 01/28/2026 13:29pm:
Water distortion vfx has been implemented

Next:
Imlpement water splash vfx.