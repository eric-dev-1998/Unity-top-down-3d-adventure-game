using Assets.Art.UI.UXML;
using Assets.Scripts.Inventory_System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Quest_System
{
    public class QuestManager : MonoBehaviour
    {
        public Quest test;

        public Quest trackedQuest;

        private Uxml_QuestMenu menu;
        private List<Quest> questDatabase;      // All quest data.
        private List<Quest> currentGameQuests;  // Only active or completed quests.

        private Inventory_System.Manager inventoryManager;

        private void Start()
        {
            questDatabase = Resources.LoadAll<Quest>("Quests").ToList();
            if (questDatabase == null || questDatabase.Count <= 0)
            {
                Debug.LogError("[Quest manager]: Quest database could not be loaded.");
                return;
            }

            inventoryManager = FindAnyObjectByType<Inventory_System.Manager>();

            menu = FindAnyObjectByType<Uxml_QuestMenu>();
            currentGameQuests = new List<Quest>();

            menu.UpdateLists(currentGameQuests);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                TriggerQuest(test);
        }

        public void SetComplete(Quest questData)
        {
            // Change quest status to completed.
            if (currentGameQuests.Contains(questData))
            {
                Quest questOnList = currentGameQuests.Find(q => q.GetId() == questData.GetId());
                if (questOnList != null)
                {
                    foreach (Objective o in questOnList.objectives)
                        o.completed = true;

                    menu.DisplayNotification(QuestNotification.NotificationType.Complete, questOnList.name);
                }
            }
        }

        public Quest FindQuest(string id)
        {
            return currentGameQuests.Find(q => q.id == id);
        }

        public Quest FindQuestData(string id)
        {
            return questDatabase.Find(q => q.id == id);
        }

        public List<Quest> GetCompletedQuests()
        {
            List<Quest> completed = new List<Quest>();

            foreach(Quest q in currentGameQuests)
                if(q.Completed())
                    completed.Add(q);

            return completed;
        }

        public List<Quest> GetCurrentQuests()
        { 
            List <Quest> current = new List<Quest>();

            foreach(Quest q in currentGameQuests)
                if(!q.Completed())
                    current.Add(q);

            return current;
        }

        public List<Quest> GetMainQuests()
        {
            List<Quest> result = new List<Quest>();

            foreach(Quest q in currentGameQuests)
                if(q.type == Quest.QuestType.MainQuest)
                    result.Add(q);

            return result;
        }

        public List<Quest> GetMainQuestsData()
        { 
            List<Quest> result = new List<Quest>();
            foreach (Quest q in questDatabase)
                if (q.type == Quest.QuestType.MainQuest)
                    result.Add(q);

            return result;
        }

        public List<Quest> GetNpcQuests(string id)
        {
            Debug.Log($"[Quest manager]: Searching quests for npc: '{id}'...");

            List<Quest> result = new List<Quest>();
            foreach (Quest q in currentGameQuests)
            { 
                if(q.from == id)
                    result.Add(q);
            }

            if (result.Count == 0)
            {
                Debug.LogError($"[Quest manager]: No quests were found for npc: '{id}'.");
                return null;
            }

            Debug.Log($"[Quest manager]: Found {result.Count} quests for npc: '{id}'.");
            return result;
        }

        public List<Quest> GetNpcQuestsData(string id)
        {
            List<Quest> result = new List<Quest>();
            foreach (Quest q in questDatabase)
            {
                if (q.from == id)
                    result.Add(q);
            }

            if (result.Count == 0)
            {
                Debug.LogError($"[Quest manager]: No quests were found in quest database for npc with id: '{id}'. Returned null.");
                return null;
            }

            return result;
        }

        public Quest GetAvailableNpcQuest(string id)
        {
            Debug.Log($"[Quest manager]: Searching for available quests for npc: '{id}'");
            
            List<Quest> npcQuests = GetNpcQuests(id);

            foreach (Quest q in questDatabase)
            {
                if (npcQuests != null)
                {
                    if (!npcQuests.Contains(q))
                    {
                        if (q.from == id)
                        {
                            if (q.requirement.Matched(q))
                            {
                                Debug.Log($"[Quest manager]: Found quest: '{q.name}'.");
                                return q;
                            }
                            else
                                Debug.LogError($"[Quest manager]: Found quest: '{q.name}' for npc with id '{id}', but its requirements are not matched.");
                        }
                    }
                }
                else
                {
                    if (q.from == id)
                    {   if (q.requirement.Matched(q))
                        {
                            Debug.Log($"[Quest manager]: Found quest: '{q.name}'.");
                            return q;
                        }
                        else
                            Debug.LogError($"[Quest manager]: Found quest: '{q.name}' for npc with id '{id}', but its requirements are not matched.");
                    }
                }
            }

            return null;
        }

        public void TriggerQuest(Quest questData)
        {
            Debug.Log("About to trigger a quest...");

            if (currentGameQuests.Contains(questData))
                return;

            // Change quest status to active.
            currentGameQuests.Add(questData);

            // Apply changes to menu list.
            menu.UpdateLists(currentGameQuests);

            // Notify:
            menu.DisplayNotification(QuestNotification.NotificationType.Activate, questData.name);

            // Check for inventory related objectives:
            foreach (Objective o in questData.objectives)
            {
                if (o.type == Objective.ObjectiveType.GetItem && inventoryManager.GetItemCount(o.targetId) >= o.maxCount)
                {
                    o.completed = true;
                    menu.DisplayNotification(QuestNotification.NotificationType.Update, questData.name);
                }
            }

            if (questData.Completed())
                menu.DisplayNotification(QuestNotification.NotificationType.Complete, questData.name);
        }

        public void ItemInventoryChanged(Item data, int newCount)
        {
            foreach (Quest q in currentGameQuests)
            {
                foreach (Objective o in q.objectives)
                {
                    if (o.type == Objective.ObjectiveType.GetItem && newCount >= o.maxCount)
                    {
                        o.completed = true;
                        if (menu.trackerOpen)
                        {
                            Debug.Log("[Quest system]: Toggling objective.");
                            Toggle tObjective = menu.GetTrackerObjectives().Find(t => t.text == o.description);
                            tObjective.value = true;
                        }
                    }
                }

                if (q.Completed())
                    menu.DisplayNotification(QuestNotification.NotificationType.Complete, q.name);
                else
                    menu.DisplayNotification(QuestNotification.NotificationType.Update, q.name);
            }

            menu.UpdateLists(currentGameQuests);
        }

        public void ReachedArea(string areaName)
        {
            // Check if any of current active quests has an area reached objective:

            foreach (Quest q in GetCurrentQuests())
            {
                foreach (Objective o in q.objectives)
                {
                    if (o.type == Objective.ObjectiveType.ReachArea && o.targetId == areaName)
                    {
                        o.completed = true;
                        if (menu.trackerOpen)
                        {
                            Toggle tObjective = menu.GetTrackerObjectives().Find(t => t.text == o.description);
                            tObjective.value = true;
                        }
                    }
                }

                if (q.Completed())
                    menu.DisplayNotification(QuestNotification.NotificationType.Complete, q.name);
                else
                    menu.DisplayNotification(QuestNotification.NotificationType.Update, q.name);
            }

            menu.UpdateLists(currentGameQuests);
        }

        public void Interacted(string objectName)
        {
            foreach (Quest q in GetCurrentQuests())
            {
                foreach (Objective o in q.objectives)
                {
                    if (o.type == Objective.ObjectiveType.Interact && o.targetId == objectName)
                    {
                        o.completed = true;
                        if (menu.trackerOpen)
                        {
                            Toggle tObjective = menu.GetTrackerObjectives().Find(t => t.text == o.description);
                            tObjective.value = true;
                        }
                    }
                }

                if (q.Completed())
                    menu.DisplayNotification(QuestNotification.NotificationType.Complete, q.name);
                else
                    menu.DisplayNotification(QuestNotification.NotificationType.Update, q.name);
            }

            menu.UpdateLists(currentGameQuests);
        }

        public void InteractedWithNPC(string id)
        {
            foreach (Quest q in GetCurrentQuests())
            {
                foreach (Objective o in q.objectives)
                {
                    if (o.type == Objective.ObjectiveType.TalkToNPC && o.targetId == id)
                    {
                        if (!o.completed)
                        {
                            o.completed = true;
                            if (menu.trackerOpen)
                            {
                                Toggle tObjective = menu.GetTrackerObjectives().Find(t => t.text == o.description);
                                tObjective.value = true;

                                if (q.Completed())
                                    menu.DisplayNotification(QuestNotification.NotificationType.Complete, q.name);
                                else
                                    menu.DisplayNotification(QuestNotification.NotificationType.Update, q.name);
                            }
                        }
                    }
                }
            }

            menu.UpdateLists(currentGameQuests);
        }
    }
}
