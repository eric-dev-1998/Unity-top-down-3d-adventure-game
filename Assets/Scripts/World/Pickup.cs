using UnityEngine;

namespace Main
{
    public class Pickup : MonoBehaviour
    {
        // Item id:
        public string itemId = "";

        private GameObject item_display;
        private bool alreadyPickedUp = false;
        private bool playerOnTrigger = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            item_display = transform.Find("Mesh").gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            if (!alreadyPickedUp)
            {
                if (Input.GetKeyDown(KeyCode.Space) && playerOnTrigger)
                    PickupItem();
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

        private void PickupItem()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                playerOnTrigger = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                playerOnTrigger = false;
        }
    }
}
