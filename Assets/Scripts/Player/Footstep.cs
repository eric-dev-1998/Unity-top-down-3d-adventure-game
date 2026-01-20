using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class Footstep : MonoBehaviour
    {
        private MainPlayer player;
        private PlayerAudio playerAudio;
        private AudioSource audio;

        private void Start()
        {
            player = FindAnyObjectByType<MainPlayer>();
            playerAudio = player.GetAudio();
            audio = GetComponent<AudioSource>();
        }

        private void MatchAudioClipWithSurface(PhysicsMaterial material)
        {
            if (player.onWater)
            {
                // Footsteps won't work while the player is on the water, unless it's not deep enough
                // in the water.
                float kneeHeight = transform.parent.position.y;
                if (kneeHeight < player.currentWaterHeight)
                {
                    audio.clip = playerAudio.footsteps["sfx_footsteps_water"];
                }

            }
            else
            {
                switch (material.name)
                {
                    case "Sand" or "Sand (Instance)":
                        audio.clip = playerAudio.footsteps["sfx_footsteps_sand"];
                        break;

                    case "Water":
                        audio.clip = playerAudio.footsteps["sfx_footsteps_water"];
                        break;

                    default:
                        audio.clip = null;
                        Debug.LogWarning("[Player][Footstep]: Unknown physics material.");
                        break;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            
            if (audio == null)
                return;

            Debug.Log($"Collider with: {other.name}");
            MatchAudioClipWithSurface(other.material);

            if (audio.isPlaying)
            {
                audio.Stop();
                audio.Play();
            }
            else
                audio.Play();
        }
    }
}
