using Assets.Scripts.Event_System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Event_System
{
    public class EventSequence : ScriptableObject
    {
        public Event_System.Event startEvent;
        public List<Event_System.Event> events = new();
    }
}
