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
            CheckOwnQuest();

            // 2. Check main quest.
            CheckMainQuest();

            // 3. Do standard dialogue.
            DoStandardDialogue();
        }

        public void CheckOwnQuest()
        {
            // Is current quest active or completed?
            QuestSet.QuestState currentQuestState = GetCurrentQuestState();

            if (currentQuestState == QuestSet.QuestState.Active)
            {
                // Changed to active.
                Debug.LogError("qwqw");
                EventSequence dialogue = Resources.Load<EventSequence>($"GameText/Dialogues/{id}/{id}_{lastQuestId}_Active_event");
                if (dialogue == null)
                {
                    Debug.LogWarning($"[Npc dialogue manager]: No dialogue at '/Resources/GameText/Dialogues/{id}/{id}_{lastQuestId}_Active_event' was found. " +
                        $"It means either the dialogue id on the resources folder doesnt match or this " +
                        $"npc is not meant to say something about this quest.");
                    return;
                }

                eventManager.StartSequence(dialogue);
                return;
            }
            else if (currentQuestState == QuestSet.QuestState.Completed)
            {
                // Changed to completed.

                EventSequence dialogue = Resources.Load<EventSequence>($"GameText/Dialogues/{id}/{id}_{lastQuestId}_Complete_event");
                if (dialogue == null)
                {
                    Debug.LogWarning($"[Npc dialogue manager]: No dialogue at 'Resources/GameText/Dialogues/{id}/{id}_{lastQuestId}_Complete_event' was found. " +
                        $"It means either the dialogue id on the resources folder doesnt match or this " +
                        $"npc is not meant to say something about this quest.");
                    return;
                }

                eventManager.StartSequence(dialogue);
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
                        return;
                }

                // Reached this point, this npc can check if there is a quest of is own
                // and notify the player it that's the case.

                Quest availableQuest = questManager.GetAvailableNpcQuest(id);
                if (availableQuest != null)
                {
                    // Tell the player a quest is available.
                    EventSequence dialogue = Resources.Load<EventSequence>($"GameText/Dialogues/{id}/{id}_{availableQuest.id}_Available_event");
                    if (dialogue == null)
                    {
                        Debug.LogWarning($"[Npc dialogue manager]: No dialogue with at 'Resources/GameText/Dialogues/{id}/{id}_{availableQuest.id}_Available_event' was found. " +
                            $"It means either the dialogue id on the resources folder doesnt match or this " +
                            $"npc is not meant to say something about this quest.");
                        return;
                    }

                    eventManager.StartSequence(dialogue);
                    lastQuestId = availableQuest.id;
                    onQuest = true;
                    return;
                }
            }

            // This npc doesnt have any quest to talk about right now.
            // Do dont do anything here.
        }

        public void CheckMainQuest()
        {
            List<Quest> mainQuests = questManager.GetMainQuests();
            if (mainQuests == null || mainQuests.Count <= 0)
            {
                // There are no main quests yet.
                return;
            }
            else
            {
                // There are main quests.
                Quest lastQuest = mainQuests[-1];
                if (lastQuest == null)
                    return;

                // This npc may have something do say about the last quest.
                if (lastQuest.id == lastMainQuestId)
                {
                    // This npc already said something about this quest, but has the quest
                    // state changed since then?

                    if (lastMainQuestState == lastQuest.Completed())
                        return;
                }

                // The quest status has changed or is a new quest so this npc may want to
                // say something about it.

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

                EventSequence dialogueSequence = Resources.Load<EventSequence>(dialogueId);
                if (dialogueSequence == null)
                {
                    Debug.LogWarning($"[Npc dialogue manager]: No dialogue with id '{dialogueId}' was found. " +
                        $"It means either the dialogue id on the resources folder doesnt match or this " +
                        $"npc is not meant to say something about this quest.");
                    return;
                }

                eventManager.StartSequence(dialogueSequence);
            }
        }

        public void DoStandardDialogue()
        {
            // Say something random.

            List<EventSequence> dialogues = Resources.LoadAll<EventSequence>($"GameText/Dialogues/{id}/Standard").ToList();
            if (dialogues == null || dialogues.Count <= 0)
            {
                Debug.LogError($"[Npc dialogue manager]: No dialogues were found for npc with id: '{id}'");
                return;
            }

            if (dialogues.Count == 1)
                eventManager.StartSequence(dialogues[0]);
            else
            {
                int rnd = Random.Range(0, dialogues.Count - 1);
                eventManager.StartSequence(dialogues[rnd]);
            }
        }

        private void StartDialogue(EventSequence sequence)
        {
            // Load the selcted event sequence and load it to the event manager.
            Event_System.EventManager eventManager = FindAnyObjectByType<Event_System.EventManager>();
            if (eventManager == null)
            {
                Debug.LogError("[Npc dialogue manager]: No event manager was found on scene.");
                return;
            }

            eventManager.StartSequence(sequence);
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
