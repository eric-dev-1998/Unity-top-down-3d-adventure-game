using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Runtime.Editor
{
    public class ConnectionSaveData : ScriptableObject
    {
        public string outputNodeId;
        public string inputNodeId;
    }
}
