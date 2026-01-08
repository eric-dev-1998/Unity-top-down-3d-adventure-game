using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Runtime.Editor.Node_properties
{
    public class DialogueLineProperties : NodeProperties
    {
        public DialogueLineProperties() { }
        public DialogueLineProperties(string author, string text)
        {
            this.lineId = text;
        }

        public string lineId = "";
    }
}
