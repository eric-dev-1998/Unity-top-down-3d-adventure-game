using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Scripts.Quest_System;
using System.Collections;
using System.Linq;
using Assets.Scripts.GameText;

namespace Assets.Art.UI.UXML
{
    public class Uxml_QuestMenu : MonoBehaviour
    {
        public bool open = false;
        public bool trackerOpen = false;

        private TextManager textManager;

        private UIDocument document;
        private UIDocument hud;

        private VisualElement menuPanel;
        private VisualElement card;

        private Foldout fActive;
        private Foldout fComplete;
        private Button buttonClose;
        private Button buttonTrack;
        private Button buttonStopTrack;
        private ListView activeList;
        private ListView completeList;
        private VisualElement noSelectedQuestMessage;
        private Label title;
        private Label nonSelected;
        private Label viewTitle;
        private Label viewDescription;
        private Label viewObjectives;

        private Button buttonCloseTracker;
        private VisualElement trackCard;
        private Label trackTitle;
        private ScrollView trackObjectivesContainer;

        private List<QuestNotification> notifications = new();

        private void Start()
        {
            document = GetComponent<UIDocument>();
            menuPanel = document.rootVisualElement.Q<VisualElement>("Root");

            title = document.rootVisualElement.Q<Label>("Title");
            noSelectedQuestMessage = document.rootVisualElement.Q<VisualElement>("NoSelectedQuestLabel");
            nonSelected = document.rootVisualElement.Q<Label>("NoSelected");
            fActive = document.rootVisualElement.Q<Foldout>("ActiveFoldout");
            fComplete = document.rootVisualElement.Q<Foldout>("CompletedFoldout");

            viewTitle = document.rootVisualElement.Q<Label>("QuestTitle");
            viewDescription = document.rootVisualElement.Q<Label>("QuestDescription");
            viewObjectives = document.rootVisualElement.Q<Label>("QuestObjectives");

            buttonClose = document.rootVisualElement.Q<Button>("ButtonClose");
            buttonClose.clicked += CloseMenu;

            buttonTrack = document.rootVisualElement.Q<Button>("ButtonTrack");
            buttonTrack.clicked += StartTrackingQuest;

            buttonStopTrack = document.rootVisualElement.Q<Button>("ButtonStopTrack");
            buttonStopTrack.clicked += StopTrackingQuest;

            activeList = document.rootVisualElement.Q<ListView>("ActiveList");
            activeList.selectionChanged += (IEnumerable<object> selectedItems) =>
            {
                if (selectedItems.Any())
                {
                    Quest quest = selectedItems.First() as Quest;
                    SelectQuest(quest);
                }
            };

            completeList = document.rootVisualElement.Q<ListView>("CompletedList");
            completeList.selectionChanged += (IEnumerable<object> selectedItems) => 
            {
                if (selectedItems.Any())
                {
                    Quest quest = selectedItems.First() as Quest;
                    SelectQuest(quest);
                }
            };

            card = GameObject.Find("HUD").GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("QuestCard");

            hud = GameObject.Find("HUD").GetComponent<UIDocument>();

            trackCard = hud.rootVisualElement.Q<VisualElement>("QuestTracker");
            trackTitle = trackCard.Q<Label>("TrackerTitle");
            trackObjectivesContainer = trackCard.Q<ScrollView>("TrackObjectives");

            buttonCloseTracker = trackCard.Q<Button>("ButtonCloseTracker");
            buttonCloseTracker.clicked += StopTrackingQuest;

            LoadText();
        }

        public void LoadText()
        {
            textManager = FindAnyObjectByType<TextManager>();
            
            if (textManager == null)
            {
                Debug.LogError("[Text manager]: No text manager was found on scene.");
                return;
            }

            buttonClose.text = textManager.GetUIText("menu_back");
            title.text = textManager.GetUIText("quest_menu_title");
            fActive.text = textManager.GetUIText("quest_menu_active");
            fComplete.text = textManager.GetUIText("quest_menu_complete");
            nonSelected.text = textManager.GetUIText("quest_menu_no_selected");
            buttonTrack.text = textManager.GetUIText("quest_menu_track");
            buttonStopTrack.text = textManager.GetUIText("quest_menu_track_stop");
        }

        public void OpenMenu()
        {
            open = true;
            menuPanel.RemoveFromClassList("panel_full_hidden");
            noSelectedQuestMessage.style.visibility = Visibility.Visible;
            activeList.ClearSelection();
        }

        public void CloseMenu()
        {
            open = false;
            menuPanel.AddToClassList("panel_full_hidden");
        }

        public void UpdateLists(List<Quest> newList)
        {
            UpdateActiveQuestList(newList);
            UpdateCompleteQuestList(newList);
        }

        public void UpdateActiveQuestList(List<Quest> newList)
        {
            List<Quest> activeOnly = new List<Quest>();
            foreach (Quest q in newList)
            { 
                if(!q.Completed())
                    activeOnly.Add(q);
            }

            activeList.itemsSource = activeOnly;
            activeList.fixedItemHeight = 64;

            activeList.makeItem = () => 
            {
                VisualElement itemRoot = new VisualElement();
                itemRoot.style.flexGrow = 0;
                itemRoot.style.flexDirection = FlexDirection.Row;

                VisualElement icon = new VisualElement();

                Label title = new Label();
                title.style.unityFontStyleAndWeight = FontStyle.Normal;
                title.style.fontSize = 32;

                itemRoot.Add(icon);
                itemRoot.Add(title);

                return itemRoot;
            };

            activeList.bindItem = (VisualElement root, int index) => 
            {
                VisualElement icon = root.Q<VisualElement>();
                Label title = root.Q<Label>();

                title.text = newList[index].name;
            };

            activeList.makeNoneElement = () => {
                var label = new Label(textManager.GetUIText("quest_menu_active_empty"));
                label.style.fontSize = 32;
                label.style.unityFontStyleAndWeight = FontStyle.Normal;
                return label;
            };

            activeList.Rebuild();
        }

        public void UpdateCompleteQuestList(List<Quest> newList)
        {
            List<Quest> completeOnly = new List<Quest>();
            foreach (Quest q in newList)
            {
                if (q.Completed())
                    completeOnly.Add(q);
            }

            completeList.itemsSource = completeOnly;
            completeList.fixedItemHeight = 64;

            completeList.makeItem = () =>
            {
                VisualElement itemRoot = new VisualElement();
                itemRoot.style.flexGrow = 0;
                itemRoot.style.flexDirection = FlexDirection.Row;

                VisualElement icon = new VisualElement();

                Label title = new Label();
                title.style.unityFontStyleAndWeight = FontStyle.Normal;
                title.style.fontSize = 32;

                itemRoot.Add(icon);
                itemRoot.Add(title);

                return itemRoot;
            };

            completeList.bindItem = (VisualElement root, int index) =>
            {
                VisualElement icon = root.Q<VisualElement>();
                Label title = root.Q<Label>();

                title.text = newList[index].name;
            };

            completeList.makeNoneElement = () => {
                var label = new Label(textManager.GetUIText("quest_menu_complete_empty"));
                label.style.fontSize = 32;
                label.style.unityFontStyleAndWeight = FontStyle.Normal;
                return label;
            };

            completeList.Rebuild();
        }

        private void SelectQuest(Quest quest)
        {
            QuestText text = textManager.GetQuest(quest.id);

            viewTitle.text = text.title;
            viewDescription.text = text.description;

            viewObjectives.text = "";
            foreach (Objective o in quest.objectives)
            {
                if(o.completed)
                    viewObjectives.text += textManager.GetUIText("quest_objective_complete") + text.objectives[quest.objectives.ToList().IndexOf(o)];
                else
                    viewObjectives.text += text.objectives[quest.objectives.ToList().IndexOf(o)];
            }

            noSelectedQuestMessage.style.visibility = Visibility.Hidden;
        }

        public void DisplayNotification(QuestNotification.NotificationType type, string questName)
        {
            string message = "";

            switch (type)
            {
                case QuestNotification.NotificationType.Activate:
                    message = "New quest accepted:";
                    break;

                case QuestNotification.NotificationType.Complete:
                    message = "Quest completed:";
                    break;

                case QuestNotification.NotificationType.Track:
                    message = "Tracking quest:";
                    break;

                case QuestNotification.NotificationType.Update:
                    message = "Update:";
                    break;
            }

            notifications.Add(new QuestNotification(questName, message));

            if (notifications.Count == 1)                
                StartCoroutine(DisplayNotificationMessage());
        }

        IEnumerator DisplayNotificationMessage()
        {
            QuestNotification notification = notifications[0];

            if (notification != null)
            {
                Label title = card.Q<Label>("CardTitle");
                Label name = card.Q<Label>("CardName");

                title.text = notification.message;
                name.text = notification.questName;

                card.RemoveFromClassList("quest-card-hidden");

                yield return new WaitForSeconds(3);

                card.AddToClassList("quest-card-hidden");

                yield return new WaitForSeconds(0.26f);

                notifications.Remove(notification);
                if (notifications.Count > 0 && notifications[0] != null)
                    yield return StartCoroutine(DisplayNotificationMessage());
            }
            
            yield break;
        }

        public List<Toggle> GetTrackerObjectives()
        {
            List<Toggle> result = new List<Toggle>();

            foreach(Toggle t in trackObjectivesContainer.contentContainer.Children())
                if(t is Toggle)
                    result.Add(t);

            return result;
        }

        private void StartTrackingQuest()
        {
            // Get selected quest:
            Quest selectedQuest = activeList.selectedItem as Quest;

            if (selectedQuest == null)
                return;

            trackTitle.text = $"Tracking quest: {selectedQuest.name}";
            trackObjectivesContainer.contentContainer.Clear();
            foreach (Objective o in selectedQuest.objectives)
            {
                Toggle toggleObjective = new Toggle();
                toggleObjective.labelElement.style.marginLeft = 8;
                toggleObjective.text = o.description;
                toggleObjective.value = o.completed;
                toggleObjective.toggleOnLabelClick = false;
                toggleObjective.SetEnabled(false);
                toggleObjective.AddToClassList("tracker-toggle");

                trackObjectivesContainer.contentContainer.Add(toggleObjective);
            }

            trackerOpen = true;
            trackCard.RemoveFromClassList("quest-tracker-hidden");
        }

        private void StopTrackingQuest()
        {
            trackerOpen = false;
            trackCard.AddToClassList("quest-tracker-hidden");
        }
    }
}
