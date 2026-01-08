using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Main
{
    public class Door : MonoBehaviour
    {
        [Header("Main properties:")]
        public bool openOnce = false;
        public SceneAsset sceneToLoad;

        [Header("Lock properties:")]
        public bool locked = false;
        public string itemId;
        public int itemCount = 1;
        [TextArea]
        public string lockedDescription;

        private float distanceToClose = 3f;
        private Animator animator;
        private bool playerOnTrigger = false;
        private bool opened = false;

        private NpcPathSystem path;

        private void Start()
        {
            animator = GetComponent<Animator>();
            path = GetComponent<NpcPathSystem>();
        }

        private void Update()
        {
            if (!opened)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    if (playerOnTrigger)
                        Open();
            }
            else
            {
                if (openOnce)
                {
                    // Close the door after player gets away if this door doesnt load another scene.
                    if (DistanceFromPlayer() > distanceToClose)
                        CloseDoor();
                }
            }
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

        private void Open()
        {
            // 1. Is is locked?
            // 1.1 Does the player have what is needed to open this door?
            // 1.2 If so, open it.
            // 1.3 If not, display locked description.
            // 2. Open door if it's not locked.

            if (locked)
            {
                if (PlayerInventory.Owns(itemId, itemCount))
                {
                    OpenDoor();
                    PlayerInventory.ConsumeItem(itemId, itemCount);
                    locked = false;
                }
                else
                    ShowLockedDescription();
            }
            else
                OpenDoor();
        }

        private void OpenDoor()
        {
            animator.SetBool("Open", true);
            opened = true;

            if (sceneToLoad != null)
                StartCoroutine(LoadScene());
        }

        private void ShowLockedDescription()
        {

        }

        private void CloseDoor()
        {
            animator.SetBool("Open", false);
            opened = false;
        }

        private float DistanceFromPlayer()
        {
            Vector3 playerPosition = GameObject.FindAnyObjectByType<MainPlayer>().transform.position;
            float distance = Vector3.Distance(transform.position, playerPosition);

            return distance;
        }

        IEnumerator MovePlayerTowardsDoor()
        {
            // Get player.
            MainPlayer player = GameObject.Find("Player").GetComponent<MainPlayer>();
            Entity playerEntity = player.GetEntity();

            // Move player.
            path.SetTargetEntity(playerEntity);
            path.StartFollowingPath(0, false);

            yield return new WaitUntil(() => !path.isFollowing());
        }

        private IEnumerator LoadScene()
        {
            // Disable collider so player can walk trough:
            try
            {
                BoxCollider collider = transform.Find("Mesh").GetComponent<BoxCollider>();
                Destroy(collider);
            }
            catch { }

            // Wait for the door to be fully opened.
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

            // Make player move towards the door and wait.
            if (path)
            {
                yield return StartCoroutine(MovePlayerTowardsDoor());
            }

            // Start fade animation and wait.
            Animator fadeAnimator = GameObject.Find("BlackScreen").GetComponent<Animator>();
            fadeAnimator.SetBool("FadeIn", true);
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => fadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

            // Load scene.
            //SceneManager.LoadScene(sceneToLoad.name, LoadSceneMode.Single);
        }
    }
}
