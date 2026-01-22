using Assets.Scripts.World.Npc;
using Assets.Scripts.Player;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Inventory_System;
using UnityEngine.SceneManagement;
using Assets.Scripts.Event_System;

namespace Assets.Scripts.World
{
    public class Door : MonoBehaviour
    {
        [Header("Main properties:")]
        public bool openOnce = false;
        public SceneAsset sceneToLoad;

        [Header("Lock properties:")]
        public bool locked = false;
        public Item neededItem;
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
            if (locked)
            {
                InventoryManager iManager = FindAnyObjectByType<InventoryManager>();
                if (iManager == null)
                {
                    Debug.LogError($"[Door('{name}')]: No inventory manager was found on scene, this door will not work.");
                    return;
                }

                if (iManager.ConsumeItem(neededItem.item_id, 1))
                {
                    OpenDoor();
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
            // Find a locked door dialogue.
            // For now, a generic dialogue will be used for every door.

            EventSequence dialogue = Resources.Load<EventSequence>("GameText/Dialogues/World/Door_Locked");
            if (dialogue != null)
            { 
                EventManager eManager = FindAnyObjectByType<EventManager>();
                if(eManager != null && !eManager.busy)
                    eManager.StartSequence(dialogue);
            }
        }

        private void CloseDoor()
        {
            animator.SetBool("Open", false);
            opened = false;
        }

        private float DistanceFromPlayer()
        {
            Vector3 playerPosition = GameObject.FindAnyObjectByType<PlayerCore>().transform.position;
            float distance = Vector3.Distance(transform.position, playerPosition);

            return distance;
        }

        IEnumerator MovePlayerTowardsDoor()
        {
            // Get player.
            PlayerCore player = GameObject.Find("Player").GetComponent<PlayerCore>();
            Entity playerEntity = player.GetEntity();

            // Move player.
            path.SetEntity(playerEntity);
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
            VFXManager vfxManager = FindAnyObjectByType<VFXManager>();
            yield return StartCoroutine(vfxManager.PlayVFX(VFXManager.VFX.Dark_FadeIn));

            // Load scene.
            SceneManager.LoadScene(sceneToLoad.name, LoadSceneMode.Single);
        }
    }
}
