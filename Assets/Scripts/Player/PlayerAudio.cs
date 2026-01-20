using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerAudio
    {
        public Dictionary<string, AudioClip> footsteps;

        public PlayerAudio()
        {
            LoadFootsteps();
        }

        private void LoadFootsteps()
        {
            var clips = Resources.LoadAll<AudioClip>("Art/Audio/Sfx/World/Footsteps");
            if (clips == null || clips.Count() <= 0)
            {
                Debug.LogError("[Player][Audio]: Footsteps audio clips could not be loaded");
                return;
            }

            footsteps = new();
            foreach (AudioClip clip in clips)
            { 
                footsteps.Add(clip.name, clip);
            }

            Debug.Log("[Player][Audio]: Footsteps audio clips were loaded.");
        }
    }
}
