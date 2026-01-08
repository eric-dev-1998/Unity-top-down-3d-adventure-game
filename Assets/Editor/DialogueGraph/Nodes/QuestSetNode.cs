using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Quest_System;
using Assets.Scripts.Runtime.Editor.Node_properties;
using Editor.DialogueGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Editor.DialogueGraph.Nodes
{
    public class QuestSetNode : Node
    {
        public QuestProperties properties => GetProperties<QuestProperties>();

        public DropdownField questId;
        public DropdownField questState;

        public QuestSetNode() { }

        public QuestSetNode(Vector2 screenPosition, DialogueGraphView parent)
        {
            name = "Set quest";
            this.parent = parent;
            this.screenPosition = screenPosition;

            CreateNode();
            CreateProperties();
            LoadVisualContent();

            AssetDatabase.AddObjectToAsset(saveData, parent.currentGraphData);
        }

        public override void LoadVisualContent()
        {
            node_data.title = "Set quest state";

            // Load node uxml fields:
            string contentPath = "Assets/Editor/DialogueGraph/uxml/quest_set.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            questId = node_data.extensionContainer.Q<DropdownField>("SelectQuest");
            var assetsInPath = AssetDatabase.FindAssets("t:Assets.Scripts.Quest_System.Quest", new[] { "Assets/Scriptable Objects/Quests" });


            Debug.Log($"[Loaded assets]: {assetsInPath.Length}");

            List<Quest> quests = new();
            foreach (var asset in assetsInPath)
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                var quest = AssetDatabase.LoadAssetAtPath<Quest>(path);
                if (quest != null)
                    quests.Add(quest);
            }
            List<string>names = quests.Select(q => q.name).ToList();
            questId.choices.Add("Select a quest...");
            foreach (string n in names)
                questId.choices.Add(n);
            questId.index = 0;

            questState = node_data.extensionContainer.Q<DropdownField>("SelectState");
            questState.choices.Add("None");
            questState.choices.Add("Active");
            questState.choices.Add("Completed");
            questState.index = 0;

            if (properties != null)
            {
                questId.value = properties.id;
                questState.value = properties.state.ToString();
            }
        }

        public override void CreateProperties()
        {
            // Create properties instance in save data for serialization:
            saveData.properties = ScriptableObject.CreateInstance<QuestProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();

            SetCallbacks();
        }

        public override void LoadProperties()
        {
            questId.value = properties.id;
            questState.value = properties.state.ToString();

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            // Register ui element callbacks to keep properties updated:
            questId.RegisterValueChangedCallback(evt => {
                properties.id = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
            questState.RegisterValueChangedCallback(evt => {
                if (evt.newValue != "Select a state...")
                    properties.state = (QuestSet.QuestState)Enum.Parse(typeof(QuestSet.QuestState), evt.newValue);
                else
                    properties.state = QuestSet.QuestState.None;
                
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
        }
    }
}
