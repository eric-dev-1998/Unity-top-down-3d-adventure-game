using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameText
{
    [Serializable]
    public class PlainText
    {
        public string id;
        [TextArea]
        public string content;
    }
}
