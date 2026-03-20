using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Systems.Character_Path
{
    public class PathContainer : MonoBehaviour
    {
        public List<Path> paths;
        private void Start()
        {
            // Stop rendering path point display meshes in game mode.
            foreach (var path in paths)
            {
                for (int i = 0; i < path.points.Count; i++)
                {
                    Transform currentPoint = path.points[i].transform;
                    currentPoint.GetComponent<MeshRenderer>().enabled = false;
                    currentPoint.GetComponent<SphereCollider>().enabled = false;
                }
            }
        }
    }
}
