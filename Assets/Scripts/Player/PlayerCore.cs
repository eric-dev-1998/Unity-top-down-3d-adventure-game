using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCore : MonoBehaviour
    {
        private Entity entity;
        private PlayerInput playerInput;
        private PlayerAudio playerAudio;

        public bool canMove = true;
        public float currentWaterHeight = 0f;

        private float velocity = 0;

        private void Awake()
        {
            entity = GetComponent<Entity>();

            if (!entity)
            {
                entity = gameObject.GetComponent<Entity>();
                if (!entity)
                    Application.Quit();
            }

            playerAudio = GetComponent<PlayerAudio>();

            playerInput = GetComponent<PlayerInput>();
            if (!playerInput)
            {
                playerInput = gameObject.GetComponent<PlayerInput>();
                if (!playerInput)
                {
                    gameObject.AddComponent<PlayerInput>();
                    playerInput = gameObject.GetComponent<PlayerInput>();
                }
            }
        }

        public Entity GetEntity()
        {
            return entity;
        }

        public PlayerAudio GetAudio()
        {
            return playerAudio;
        }
    }
}
