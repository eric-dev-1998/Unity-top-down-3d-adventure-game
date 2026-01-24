using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class Footstep : MonoBehaviour
    {
        private PlayerCore player;
        private PlayerAudio playerAudio;
        private new AudioSource audio;

        private void Start()
        {
            player = FindAnyObjectByType<PlayerCore>();
            playerAudio = player.GetAudio();
            audio = GetComponent<AudioSource>();
        }

        
    }
}
