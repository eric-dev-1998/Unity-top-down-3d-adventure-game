using Assets.Scripts.Dialogue_System;
using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Event_System;
using Assets.Scripts.GameText;
using Assets.Scripts.Quest_System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.World.Npc
{
    /*
        Each time a npc needs to start a dialogue, it will do so by loading a
        event sequence from the resources folder right after some validation
        steps to determine which dialogue is required.
     */

    public class NpcDialogue : MonoBehaviour
    {
        private Quest_System.QuestManager questManager;
        private Event_System.EventManager eventManager;
        private TextManager textManager;

        private string id;
        private string lastQuestId = "";
        private string lastMainQuestId = "";

        private bool greeted = false;
        private bool onQuest = false;
        private bool talkedAboutOwnQuest = false;
        private bool talkedAboutMainQuest = false;
        private bool questAvailable = false;
        private bool lastQuestState = false;
        private bool lastMainQuestState = false;

        private Quest lastQuest;
        private NpcQuestText currentOwnQuest;
        private NpcQuestText nextOwnQuest;
        private NpcQuestText currentMainQuest;

        public void Load(string id)
        {
            this.id = id;

            // Load quest manager to access actual quest list to check status.
            questManager = FindAnyObjectByType<Quest_System.QuestManager>();
            if (questManager == null)
            {
                Debug.LogError("[Npc dialogue manager]: No quest manager was found on scene.");
                return;
            }

            // Load npc dialogues text.
            // This will be loaded once so it its omptimum.
            textManager = FindAnyObjectByType<TextManager>();
            if (textManager == null)
            {
                Debug.LogError($"[Npc dialogue manager]: No text manager was found on scene. No text for this npc with id: '{id}' could be loaded.");
                return;
            }

            eventManager = FindAnyObjectByType<Event_System.EventManager>();
            if (eventManager == null)
            {
                Debug.LogError("[Npc dialogue manager]: No event manager was found on scene.");
                return;
            }
        }

        public void TriggerDialogue()
        {
            questManager.InteractedWithNPC(id);
            eventManager.OnEventFinished += GetComponent<NPC>().OnSequenceEnd;

            // 1. Check own quest.
            if (CheckOwnQuest())
                return;

            // 2. Check main quest.
            if (CheckMainQuest())
                return;

            // 3. Do standard dialogue.
            DoStandardDialogue();
        }

        public bool CheckOwnQuest()
        {
            // Is current quest active or completed?
            QuestSet.QuestState currentQuestState = GetCurrentQuestState();

            if (currentQuestState == QuestSet.QuestState.Active)
            {
                // Changed to active.
                EventSequence dialogue = Resources.Load<EventSequence>($"GameText/Dialogues/{id}/{lastQuestId}/active");
                if (dialogue == null)
                {
                    Debug.LogWarning($"[Npc dialogue manager]: No dialogue at '/Resources/GameText/Dialogues/{id}/{lastQuestId}/active' was found. " +
                        $"It means either the dialogue id on the resources folder doesnt match or this " +
                        $"npc is not meant to say something about this quest.");
                    return false;
                }

                eventManager.StartSequence(dialogue);
                return true;
            }
            else if (currentQuestState == QuestSet.QuestState.Completed)
            {
                // Changed to completed.

                EventSequence dialogue = Resources.Load<EventSequence>($"GameText/Dialogues/{id}/{lastQuestId}/complete");
                if (dialogue == null)
                {
                    Debug.LogWarning($"[Npc dialogue manager]: No dialogue at 'Resources/GameText/Dialogues/{id}/{lastQuestId}/complete' was found. " +
                        $"It means either the dialogue id on the resources folder doesnt match or this " +
                        $"npc is not meant to say something about this quest.");
                    return false;
                }

                eventManager.StartSequence(dialogue);
                return true;
            }
            else
            {
                // Quest state is 'None' which could mean two posibilities:

                // 1. There is no 'last quest', this means this npc has no record of
                //    any owned quest beign active or completed.

                // 2. There is a 'last quest' and its active, but its status haven't
                //    changed.

                // Check if a quest was previously checked:
                if (!string.IsNullOrEmpty(lastQuestId))
                {
                    // There is a 'last quest':

                    Quest q = questManager.FindQuest(lastQuestId);

                    // Return if 'last quest' is not completed yet.
                    if (q != null && !q.Completed())
                        return false;
                }

                // Reached this point, this npc can check if there is a quest of is own
                // and notify the player it that's the case.

                Quest availableQuest = questManager.GetAvailableNpcQuest(id);
                if (availableQuest != null)
                {
                    // Tell the player a quest is available.
                    EventSequence dialogue = Resources.Load<EventSequence>($"GameText/Dialogues/{id}/{availableQuest.id}/available");
                    if (dialogue == null)
                    {
                        Debug.LogWarning($"[Npc dialogue manager]: No dialogue with at 'Resources/GameText/Dialogues/{id}/{availableQuest.id}/available' was found. " +
                            $"It means either the dialogue id on the resources folder doesnt match or this " +
                            $"npc is not meant to say something about this quest.");
                        return false;
                    }

                    eventManager.StartSequence(dialogue);
                    lastQuestId = availableQuest.id;
                    onQuest = true;
                    return true;
                }

                return false;
            }

            // This npc doesnt have any quest to talk about right now.
            // Do dont do anything here.
        }

        public bool CheckMainQuest()
        {
            // Get current quests: this inlcudes only 'active' and 'completed' quests.
            List<Quest> mainQuests = questManager.GetMainQuests();

            if (mainQuests == null || mainQuests.Count <= 0)
            {
                // No active or completed quests have beend found, so there is nothing to say about that.
                // But, there may be some quests avaliable, so let's see:

                // Look for any available main quest:
                Quest nextAvailableQuest = questManager.GetAvailableMainQuest();

                if (nextAvailableQuest != null)
                {
                    // An available quest was found. So let's check:

                    if (lastMainQuestId == nextAvailableQuest.id)
                    {
                        // This npc already said something about this npc.
                        Debug.Log($"[Npc dialogue]: Already said something about this available quest.\n'{lastMainQuestId}' == '{nextAvailableQuest.id}'");
                        return false;
                    }

                    // This npc haven't said anything about this available, so let them say something.
                    lastMainQuestId = nextAvailableQuest.id;

                    // Start the dialogue.
                    EventSequence dialogue = Resources.Load<EventSequence>($"GameText/Dialogues/{id}/{nextAvailableQuest.id}/available");
                    if (dialogue == null)
                    {
                        // If this npc instance has no dialogue linked to te available main quests, it just wont do anything as
                        // this is expected to happen in some cases, npc's are not intended to talk about every quest.

                        Debug.LogWarning($"[Npc dialogue]: No event sequence found at: 'GameText/Dialogues/{id}/{nextAvailableQuest.id}/available'," +
                            $"either the asset is missing or this npc is not intended to say something about this availble quest.");
                        
                        return false;
                    }

                    eventManager.StartSequence(dialogue);
                    return true;
                }

                return false;
            }
            else
            {
                // There are active and/or complete main quests. So let's check:

                // Get the latest quest added. This doesnt mean the last updated quest. It means
                // the latest quest added to the list, the order on that list should not change.
                Quest lastQuest = mainQuests[mainQuests.Count - 1];
                if (lastQuest == null)
                    return false;

                // This npc may have something do say about the last quest.
                if (lastQuest.id == lastMainQuestId)
                {
                    // This npc already said something about this quest, but has the quest
                    // state changed since then?

                    Debug.Log("[Npc dialogue]: This npc already talked about this main quest.");

                    if (lastMainQuestState == lastQuest.Completed())
                    {
                        Debug.Log("But nothing has changed since then.");
                        return false;
                    }
                    else
                        Debug.Log("But something changed.");
                }

                // The quest status either changed or it is a new quest so this npc may want to
                // say something about it.

                lastMainQuestId = lastQuest.id;
                lastMainQuestState = lastQuest.Completed();

                string dialogueId = "";

                if (lastQuest.Completed())
                {
                    // Say something about the completed quest
                    dialogueId = $"{id}_{lastQuest.id}_complete";
                }
                else
                {
                    // Say something about the active quest.
                    dialogueId = $"{id}_{lastQuest.id}_active";
                }

                Debug.Log("[Dialogue npc]: A npc dalogue about the recently updated quest is about to start.");

                EventSequence dialogueSequence = Resources.Load<EventSequence>(dialogueId);
                if (dialogueSequence == null)
                {
                    Debug.LogWarning($"[Npc dialogue manager]: No dialogue with id '{dialogueId}' was found. " +
                        $"It means either the dialogue id on the resources folder doesnt match or this " +
                        $"npc is not meant to say something about this quest.");
                    return false;
                }

                eventManager.StartSequence(dialogueSequence);
                return true;
            }
        }

        public bool DoStandardDialogue()
        {
            // Say something random.

            List<EventSequence> dialogues = Resources.LoadAll<EventSequence>($"GameText/Dialogues/{id}/Standard").ToList();
            if (dialogues == null || dialogues.Count <= 0)
            {
                Debug.LogError($"[Npc dialogue manager]: No dialogues were found for npc with id: '{id}'");
                return false;
            }

            if (dialogues.Count == 1)
                eventManager.StartSequence(dialogues[0]);
            else
            {
                int rnd = Random.Range(0, dialogues.Count);
                eventManager.StartSequence(dialogues[rnd]);
            }

            return true;
        }

        public QuestSet.QuestState GetCurrentQuestState()
        {
            if(!onQuest)
                return QuestSet.QuestState.None;

            Quest lastQuest = questManager.FindQuest(lastQuestId);
            if (lastQuest == null)
                return QuestSet.QuestState.None;

            if (lastQuest.Completed())
            {
                onQuest = false;
                return QuestSet.QuestState.Completed;
            }
            else
                return QuestSet.QuestState.Active;
        }
    }
}
