using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Runtime.Editor.Node_properties
{
    public class AnimationProperties : NodeProperties
    {
        public string objectName;
        public string booleanName;
        public bool booleanValue;

        public AnimationProperties() 
        {
            objectName = string.Empty;
            booleanName = string.Empty;
            booleanValue = false;
        }
    }
}
