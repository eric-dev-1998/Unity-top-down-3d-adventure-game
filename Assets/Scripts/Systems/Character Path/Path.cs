using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Systems.Character_Path
{
    [System.Serializable]
    public class Point
    {
        public Transform transform;
        public enum EntityMotion
        {
            Ignore, 
            Stop, 
            Walk, 
            Run
        };

        public EntityMotion motion;

        public Point(Transform transform)
        {
            this.transform = transform;
        }
    }

    [System.Serializable]
    public class Path
    {
        public string name = "New path";
        public List<Point> points = new List<Point>();
    }
}
