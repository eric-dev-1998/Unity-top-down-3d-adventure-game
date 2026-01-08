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
    public class QuestState : Node
    {
        QuestProperties properties => GetProperties<QuestProperties>();

        public DropdownField questName;
        public DropdownField questState;

        public QuestState() { }

        public QuestState(Vector2 screenPosition, DialogueGraphView parent)
        {
            name = "Get quest state";
            this.parent = parent;
            this.screenPosition = screenPosition;

            CreateNode();
            CreateProperties();
            LoadVisualContent();

            AssetDatabase.AddObjectToAsset(saveData, parent.currentGraphData);
        }

        public override void LoadVisualContent()
        {
            node_data.title = "Get quest state";

            MakeTrueOrFalseOuputOnly();

            // Load node uxml fields:
            string contentPath = "Assets/Editor/DialogueGraph/uxml/quest_state.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            questName = node_data.extensionContainer.Q<DropdownField>("QuestName");
            questState = node_data.extensionContainer.Q<DropdownField>("QuestState");

            // Fill quest dropdown list:
            var assetsInPath = AssetDatabase.FindAssets("t:Assets.Scripts.Quest_System.Quest", new[] { "Assets/Scriptable Objects/Quests" });

            List<Quest> quests = new();
            foreach (var asset in assetsInPath)
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                var quest = AssetDatabase.LoadAssetAtPath<Quest>(path);
                if (quest != null)
                    quests.Add(quest);
            }

            List<string> names = quests.Select(q => q.name).ToList();

            questName.choices.Add("Select a quest...");
            foreach (string n in names)
                questName.choices.Add(n);

            questName.index = 0;

            // Apply properties if they exsist:
            if (properties != null)
            {
                questName.value = properties.id;
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
            questName.value = properties.id;
            questState.value = properties.state.ToString();

            SetCallbacks();
        }

        public void SetCallbacks()
        {
            // Register ui element callbacks to keep properties updated:
            questName.RegisterValueChangedCallback(evt => {
                properties.id = evt.newValue;
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });

            questState.RegisterValueChangedCallback(evt => 
            {
                properties.state = (QuestSet.QuestState)Enum.Parse(typeof(QuestSet.QuestState), evt.newValue);
                EditorUtility.SetDirty(saveData);
                EditorUtility.SetDirty(properties);
            });
        }
    }
}
