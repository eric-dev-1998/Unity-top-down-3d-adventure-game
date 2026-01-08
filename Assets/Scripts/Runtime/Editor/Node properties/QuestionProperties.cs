using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Runtime.Editor.Node_properties
{
    public class QuestionProperties : NodeProperties
    {
        public string author = "";
        public string question = "";
        public string optionA = "";
        public string optionB = "";

        public QuestionProperties() 
        {
            author = string.Empty;
            question = string.Empty;
            optionA = string.Empty;
            optionB = string.Empty;
        }
    }
}
