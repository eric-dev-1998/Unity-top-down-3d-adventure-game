using UnityEngine;
using System.Collections;
using Assets.Scripts.Event_System;
using Assets.Scripts.Dialogue_System;
using Assets.Scripts.World.Npc;
using System;

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
			NpcPathSystem pathSystem;
			Entity entity;

			try
			{
				pathSystem = eManager.Find("Paths").GetComponent<NpcPathSystem>();
				entity = eManager.Find(who).GetComponent<Entity>();
			}
			catch (Exception e) 
			{
				UnityEngine.Debug.LogError($"[Event manager][Follow path]: Either Path system or target entity could not be found on scene, event aborted:\n\n{e}");
				yield break;
			}

			yield return pathSystem.SetAndFollow(entity, path);

			if (next != null && next.Count > 0)
				if (next[0] != null)
					next[0].Process(eManager, dManager);
        }
	}
}