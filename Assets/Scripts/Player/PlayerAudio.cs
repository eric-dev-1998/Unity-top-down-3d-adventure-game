using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerAudio : MonoBehaviour
    {
        private PlayerCore core;
        public Dictionary<string, AudioClip> footsteps;

        private AudioSource footstep;

        private void Start()
        {
            core = GetComponent<PlayerCore>();
            LoadFootsteps();
        }

        private void LoadFootsteps()
        {
            footstep = transform.Find("Sfx/Footstep").GetComponent<AudioSource>();
            if (!footstep)
            {
                Debug.LogError("[Player][Audio]: Footstep audio source was not found on scene.");
                return;
            }

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

        public void OnFootstep()
        {
            if (core.GetEntity().onWater)
            {
                if (core.GetEntity().onDeepWater)
                    footstep.PlayOneShot(footsteps["sfx_footsteps_water_deep"]);
                else
                    footstep.PlayOneShot(footsteps["sfx_footsteps_water"]);
            }
            else
            {
                PhysicsMaterial groundMaterial = core.GetEntity().GetGroundSurfaceMaterial();

                if (!groundMaterial)
                {
                    Debug.LogWarning("[Player][Audio]: No ground material was detected.");
                    return;
                }

                switch (groundMaterial.name)
                {
                    case "Sand" or "Sand (Instance)":
                        footstep.PlayOneShot(footsteps["sfx_footsteps_sand"]);
                        break;

                    case "Water" or "Water (Instace)":
                        footstep.PlayOneShot(footsteps["sfx_footsteps_water"]);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
