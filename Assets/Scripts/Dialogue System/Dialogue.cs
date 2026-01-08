using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Dialogue_System
{
    [Serializable]
    public class Dialogue
    {
        public string author = "";
        public bool displayIcon = false;
        public string iconPath;

        public List<DialogueLine> dialogueLines;
        public List<string> lines_data;

        public bool Build()
        {
            return false;
        }
    }

    [Serializable]
    public class DialogueLine
    {
        [TextArea]
        public string text = "";

        public bool isQuestion = false;
        public bool center = false;

        public string answer_a_label = "";
        public string answer_b_label = "";

        public string answer_a_sequence_data;
        public string answer_b_sequence_data;

        public DialogueLine() {}

        public DialogueLine(string text, bool isQuestion, string answerA, string answerB)
        {
            this.text = text;
            this.isQuestion = isQuestion;

            if (this.isQuestion)
            {
                answer_a_label = answerA;
                answer_b_label = answerB;
            }
        }
    }
}
