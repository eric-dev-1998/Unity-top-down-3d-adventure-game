using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameText
{
    [Serializable]
    public class ItemText
    {
        public string id;
        public string name;
        [TextArea]
        public string description;
    }
}
