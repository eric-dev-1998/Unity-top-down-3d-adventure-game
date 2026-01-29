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

        private float lastStepTime = 0f;
        private float stepCooldown = 0.15f;

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

        public void OnLeftFootstep()
        {
            OnFootstep(true);
        }

        public void OnRightFootstep()
        {
            OnFootstep(false);
        }

        public void OnFootstep(bool left)
        {
            if (!GetComponent<CharacterController>().isGrounded)
                return;

            if (core.GetEntity().currentVelocity.magnitude < 0.1f)
                return;

            /*
            if (Time.time - lastStepTime < stepCooldown)
                return;

            lastStepTime = Time.time;
            */

            if (core.GetEntity().onWater)
            {
                if (core.GetEntity().onDeepWater)
                    footstep.PlayOneShot(footsteps["sfx_footsteps_water_deep"]);
                else
                {
                    footstep.PlayOneShot(footsteps["sfx_footsteps_water"]);
                    ParticleSystem leftSplash = transform.Find("Armature/Hips/LeftLeg/LeftKnee/LeftAnkle/Water_Splash").GetComponent<ParticleSystem>();
                    ParticleSystem rightSplash = transform.Find("Armature/Hips/RightLeg/RightKnee/RightAnkle/Water_Splash").GetComponent<ParticleSystem>();

                    leftSplash.transform.position = new Vector3(
                        leftSplash.transform.position.x,
                        core.GetEntity().currentWaterBodyHeight,
                        leftSplash.transform.position.z);

                    rightSplash.transform.position = new Vector3(
                        rightSplash.transform.position.x,
                        core.GetEntity().currentWaterBodyHeight,
                        rightSplash.transform.position.z);

                    if (leftSplash == null || rightSplash == null)
                    {
                        Debug.LogError("[Player][VFX]: No splash particle vfx was found.");
                        return;
                    }

                    if (left)
                    {
                        if (!leftSplash.isPlaying)
                            leftSplash.Play();

                        leftSplash.Emit(1);
                    }
                    else
                    {
                        if (!rightSplash.isPlaying)
                            rightSplash.Play();

                        leftSplash.Emit(1);
                    }

                    Debug.Log("Emitted water splash.");
                }
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
                        if (left)
                            transform.Find("Armature/Hips/LeftLeg/LeftKnee/LeftAnkle/Sand").GetComponent<ParticleSystem>().Emit(1);
                        else
                            transform.Find("Armature/Hips/RightLeg/RightKnee/RightAnkle/Sand").GetComponent<ParticleSystem>().Emit(1);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
