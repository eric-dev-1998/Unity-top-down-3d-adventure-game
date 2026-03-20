using UnityEngine;
using System.Collections;
using Assets.Scripts.Event_System;
using Assets.Scripts.Dialogue_System;
using Assets.Scripts.World.Npc;
using System;
using Assets.Scripts.Systems.Character_Path;
using System.Linq;

namespace Assets.Scripts.Event_system.Events
{
	public class FollowPath: Event_System.Event
	{

		public string who = "";
		public string path = "";
		public bool sync = false;

		public FollowPath() { }
        public override IEnumerator Process(EventManager eManager, Manager dManager)
        {
			UnityEngine.Debug.Log($"{who} is following...");
			UnityEngine.Debug.Log($"next.count = {next.Count}");

			PathHandler handler;

			try
			{
				// Find the entity and it's handler:
				Entity entity = eManager.Find(who).GetComponent<Entity>();
				handler = entity.GetComponent<PathHandler>();

				if (handler == null)
				{
					UnityEngine.Debug.LogError($"[Event manager][Follow path]: No path handler was found for {who}. Event aborted.");
					yield break;
				}
			}
			catch (Exception e)
			{
                UnityEngine.Debug.LogError($"[Event manager][Follow path]: Event could not be started: \n\n{e}");
				yield break;
            }

			if (sync)
				yield return handler.SetAndFollow(path);
			else
				eManager.StartCoroutine(handler.SetAndFollow(path));

			if (next != null && next.Count != 0)
				if (next[0] != null)
					yield return eManager.StartCoroutine(next[0].Process(eManager, dManager));
        }
	}
}