using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Runtime.Editor.Node_properties
{
    public class MultilineDialogueProperties : NodeProperties
    {
        public string author = "";
        public List<string> lines;

        public MultilineDialogueProperties() 
        {
            lines = new List<string>();
        }

        public MultilineDialogueProperties(string author, List<string> lines)
        { 
            this.author = author;
            this.lines = lines;
        }
    }
}
