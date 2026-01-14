using Assets.Scripts.Runtime.Editor;
using Editor.DialogueGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Assets.Editor.DialogueGraph
{
    public static class DialogueGraphAssetOpener
    {
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var asset = EditorUtility.EntityIdToObject(instanceId) as DialogueGraphData;
            if(asset == null)
                return false;

            DialogueGraphEditor.Open(asset);
            return true;
        }
    }
}
