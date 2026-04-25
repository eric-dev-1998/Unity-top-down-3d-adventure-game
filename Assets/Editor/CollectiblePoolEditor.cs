using Assets.Scripts.World;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [CustomEditor(typeof(CollectiblePool))]
    public class CollectiblePoolEditor : UnityEditor.Editor
    {
        CollectiblePool _pool;

        private void OnEnable()
        {
            _pool = (CollectiblePool)target;

            if (!_pool.transform.Find("Life stones"))
            {
                GameObject container = new GameObject("Life stones");
                container.transform.parent = _pool.transform;
                container.name = "Life stones";
            }

            if (!_pool.transform.Find("Magic crystals"))
            {
                GameObject container = new GameObject("Magic crystals");
                container.transform.parent = _pool.transform;
                container.name = "Magic crystals";
            }

            if (!_pool.transform.Find("Spirits"))
            {
                GameObject container = new GameObject("Spirits");
                container.transform.parent = _pool.transform;
                container.name = "Spirits";
            }

            if (!_pool.transform.Find("Power orbs"))
            {
                GameObject container = new GameObject("Power orbs");
                container.transform.parent = _pool.transform;
                container.name = "Power orbs";
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill pool"))
            {
                Refill();
            }
        }

        private void Refill()
        {
            List<Collectible> collectiblesOnScene = FindObjectsByType<Collectible>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

            if (collectiblesOnScene == null || collectiblesOnScene.Count <= 0)
            {
                EditorUtility.DisplayDialog("No collectibles found", "No collectibles were found on the current scene. Make sure your GameObjects have a collectible component.", "Ok");
                return;
            }

            // Fill pool array.
            _pool.Collectibles = collectiblesOnScene.ToArray();

            foreach (Collectible c in collectiblesOnScene)
            {
                switch (c.Type)
                {
                    case Collectible.CollectibleType.Life_Stone:
                        c.transform.parent = _pool.transform.Find("Life stones");
                        break;

                    case Collectible.CollectibleType.Magic_Crystal:
                        c.transform.parent = _pool.transform.Find("Magic crystals");
                        break;

                    case Collectible.CollectibleType.Spirit:
                        c.transform.parent = _pool.transform.Find("Spirits");
                        break;

                    case Collectible.CollectibleType.Power_Orb:
                        c.transform.parent = _pool.transform.Find("Power orbs");
                        break;
                }
            }

            Debug.Log("Filled collectibles pool.");
        }
    }
}
