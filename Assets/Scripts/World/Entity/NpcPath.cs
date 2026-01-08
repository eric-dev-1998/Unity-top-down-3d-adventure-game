using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    [System.Serializable]
    public class  PathPoint
    {
        public Transform transform;
        public enum EntityMotion
        {
            Ignore, Stop, Walk, Run
        };
        public EntityMotion motion;

        public PathPoint(Transform transform)
        { 
            this.transform = transform;
        }
    }

    [System.Serializable]
    public class NpcPath
    {
        public string pathName = "New path";
        public List<PathPoint> pathPoints = new List<PathPoint>();
    }
}
