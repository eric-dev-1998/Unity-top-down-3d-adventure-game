using Assets.Scripts.Runtime.Editor.Node_properties;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DialogueGraph.Nodes
{
    public class Multiline : Node
    {
        public MultilineDialogueProperties properties => GetProperties<MultilineDialogueProperties>();

        private Foldout foldout;
        private ListView linesList;

        public Multiline() { }

        public Multiline(DialogueGraphView parent, Vector2 screenPosition)
        {
            name = "Multiline";
            this.parent = parent;
            this.screenPosition = screenPosition;

            CreateNode();
            CreateProperties();
            LoadVisualContent();

            AssetDatabase.AddObjectToAsset(saveData, parent.currentGraphData);
        }

        public override void CreateProperties()
        {
            // Create properties instance in save data for serialization:
            saveData.properties = ScriptableObject.CreateInstance<MultilineDialogueProperties>();
            saveData.typeName = this.GetType().AssemblyQualifiedName;

            base.CreateProperties();
        }

        public override void LoadProperties()
        {
            
        }

        public override void LoadVisualContent()
        {
            node_data.title = "Multiline";

            // Load node uxml fields:
            string contentPath = "Assets/Editor/DialogueGraph/uxml/multiline.uxml";
            VisualTreeAsset content = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(contentPath);
            visual_content = content.Instantiate();

            base.LoadVisualContent();

            // Get loaded node fields:
            foldout = visual_content.Q<Foldout>("LinesList");
            linesList = visual_content.Q<ListView>("List");

            Button button_add = visual_content.Q<Button>("Button_Add");
            button_add.clicked += AddTextField;

            if (properties != null)
            {
                RefreshLinesList();
            }
        }

        // Field callbacks:
        private void AddTextField()
        {
            if (properties.lines == null)
                properties.lines = new List<string>();

            properties.lines.Add("");
            RefreshLinesList();
            
        }

        private void RemoveTextField(int index)
        {
            properties.lines.RemoveAt(index);
            RefreshLinesList();
        }

        private void RefreshLinesList()
        {
            Func<VisualElement> makeItem = () =>
            {
                TextField newLine = new TextField();
                newLine.textEdition.placeholder = $"New line...";
                newLine.multiline = true;

                Button button_remove = new Button();
                button_remove.text = "Remove";

                VisualElement container = new VisualElement();
                container.style.flexDirection = FlexDirection.Row;
                container.Add(newLine);
                container.Add(button_remove);

                return container;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                TextField text = e.Q<TextField>();
                if(properties != null)
                    text.value = properties.lines[i];
                text.RegisterValueChangedCallback((evt) => 
                { 
                    properties.lines[i] = evt.newValue;
                    EditorUtility.SetDirty(saveData);
                    EditorUtility.SetDirty(saveData.properties);
                });

                Button remove = e.Q<Button>();
                remove.clicked += (() => { RemoveTextField(i); });
            };

            linesList.itemsSource = properties.lines;
            linesList.bindItem = bindItem;
            linesList.makeItem = makeItem;
            linesList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            linesList.Rebuild();

            foldout.text = $"Dialogue lines: {properties.lines.Count}";
            foldout.value = true;

            EditorUtility.SetDirty(saveData);
            EditorUtility.SetDirty(saveData.properties);
        }
    }
}
