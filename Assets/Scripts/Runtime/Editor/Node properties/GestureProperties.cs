using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Runtime.Editor.Node_properties
{
    public class GestureProperties : NodeProperties
    {
        public enum HumanGestureType { Talk, SayYes, SayNo };
        public HumanGestureType gestureType;

        public string characterName;

        public bool waitUntilFinish = true;
    }
}
