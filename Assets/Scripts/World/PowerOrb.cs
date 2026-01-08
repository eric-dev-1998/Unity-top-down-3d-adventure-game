using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.World
{
    public class PowerOrb : MonoBehaviour
    {
        public enum Power { Wind, Earth, Water, Fire, Light, Dark }
        public Power power;

        [TextArea]
        public string[] pickupDialogue;

        private PickupCollider _collider;

        private AudioSource sound;

        private bool onCollision = false;
        private bool alreadyPickedUp = false;

        private void Start()
        {
            //_collider = new PickupCollider(gameObject, PickupCollider.PickupDirection.Any);
            sound = transform.Find("SFX").GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!alreadyPickedUp)
            {
                if (onCollision)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        StartSequence();
                        UnlockPower();
                    }
                }
            }
            else
            { 
                // Play player gather animation.
                Animator playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
                AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("Gathering"))
                {   
                    // Make object invisible when player hand reaches.
                    if (stateInfo.normalizedTime >= 0.33f)
                        transform.Find("Mesh").gameObject.SetActive(false);

                    // Return player state to default after animation is finished.
                    if (stateInfo.normalizedTime >= 1)
                        playerAnimator.SetBool("Action/Gather", false);
                }
            }
        }

        private void StartSequence()
        {
            
        }

        private void UnlockPower()
        { 
            PlayerPowers playerPowers = FindAnyObjectByType<PlayerPowers>();
            
            switch (power)
            {
                case Power.Wind:
                    playerPowers.UnlockPower(0);
                    break;

                case Power.Earth:
                    playerPowers.UnlockPower(1);
                    break;

                case Power.Water:
                    playerPowers.UnlockPower(2);
                    break;

                case Power.Fire:
                    playerPowers.UnlockPower(3);
                    break;

                case Power.Light:
                    playerPowers.UnlockPower(4);
                    break;

                case Power.Dark:
                    playerPowers.UnlockPower(5);
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                onCollision = true;
        }
    }
}
