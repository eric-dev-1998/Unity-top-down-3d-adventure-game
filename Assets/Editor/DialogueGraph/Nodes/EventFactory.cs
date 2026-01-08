using Assets.Editor.DialogueGraph.Nodes;
using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Event_System;
using Assets.Scripts.Event_System.Events;
using Assets.Scripts.Runtime.Editor;
using Assets.Scripts.Runtime.Editor.Node_properties;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

namespace EventSystem
{
    public static class EventFactory
    {
        public static Assets.Scripts.Event_System.Event CreateEvent(NodeSaveData saveData)
        {
            string nodeType = saveData.typeName.Split(',')[0];

            UnityEngine.Debug.Log($"Node type found: {nodeType}");

            switch (nodeType)
            {
                case "Editor.DialogueGraph.Nodes.Line":
                    var p = saveData.properties as DialogueLineProperties;
                    if (p == null)
                        throw new System.Exception("Dialogue line properties is missing.");

                    var evt = ScriptableObject.CreateInstance<SingleLine>();
                    evt.text = p.lineId;

                    return evt;

                case "Editor.DialogueGraph.Nodes.Multiline":
                    var p1 = saveData.properties as MultilineDialogueProperties;
                    if (p1 == null)
                        throw new System.Exception("Multiline properties are missing.");

                    var evt1 = ScriptableObject.CreateInstance<Multiline>();
                    evt1.author = p1.author;
                    evt1.lines = p1.lines;

                    return evt1;

                case "Assets.Editor.DialogueGraph.Nodes.Question":
                    var p2 = saveData.properties as QuestionProperties;
                    if (p2 == null)
                        throw new System.Exception("Question properties doesnt exist.");

                    var evt2 = ScriptableObject.CreateInstance<Assets.Scripts.Event_system.Events.Question>();
                    evt2.author = p2.author;
                    evt2.question = p2.question;
                    evt2.optionA = p2.optionA;
                    evt2.optionB = p2.optionB;

                    return evt2;

                case "Assets.Editor.DialogueGraph.Nodes.Item":
                    var p3 = saveData.properties as ItemProperties;
                    if(p3 == null)
                        throw new System.Exception("Item properties doesn't exist.");

                    var evt3 = ScriptableObject.CreateInstance<ItemEvent>();
                    evt3.type = p3.eventType;
                    evt3.id = p3.id;
                    evt3.count = p3.count;

                    return evt3;

                case "Assets.Editor.DialogueGraph.Nodes.QuestSetNode":
                    var p4 = saveData.properties as QuestProperties;
                    if (p4 == null)
                        throw new System.Exception("Item properties doesn't exist.");

                    var evt4 = ScriptableObject.CreateInstance<QuestSet>();
                    evt4.questName = p4.id;
                    evt4.questState = p4.state;

                    return evt4;

                case "Assets.Editor.DialogueGraph.Nodes.QuestState":
                    var p5 = saveData.properties as QuestProperties;
                    if (p5 == null)
                        throw new System.Exception("Item properties doesn't exist.");

                    var evt5 = ScriptableObject.CreateInstance<QuestGet>();
                    evt5.questName = p5.id;
                    evt5.questState = p5.state;

                    return evt5;

                case "Assets.Editor.DialogueGraph.Nodes.GameObject":
                    var p6 = saveData.properties as GameObjectProperties;
                    if (p6 == null)
                        throw new System.Exception("Item properties doesn't exist,");

                    var evt6 = ScriptableObject.CreateInstance<GameObjectEvent>();
                    evt6.name = p6.name;
                    evt6.type = p6.type;

                    return evt6;
            }

            UnityEngine.Debug.Log("Couldnt match any event case.");
            return null;
        }
    }
}
