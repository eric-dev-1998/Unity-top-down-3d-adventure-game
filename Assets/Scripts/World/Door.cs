using Assets.Scripts.World.Npc;
using Assets.Scripts.Player;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Inventory_System;
using UnityEngine.SceneManagement;
using Assets.Scripts.Event_System;
using Assets.Scripts.Event_System.Events;

namespace Assets.Scripts.World
{
    public class Door : Toggable
    {
        [Header("Main properties:")]
        public string Id = string.Empty;
        public bool OpenOnce = false;
        public BoxCollider[] Colliders;
        public SceneAsset SceneToLoad;

        [Header("Lock properties:")]
        public bool IsLocked = false;
        public Item NeededItem;
        public int ItemCount = 1;

        private float _distanceToClose = 2f;
        private Animator _animator;
        private bool _playerOnTrigger = false;
        private bool _IsOpen = false;
        private bool _WasToggled = false;

        private NpcPathSystem path;

        private void Start()
        {
            if (string.IsNullOrEmpty(Id))
            {
                // Disable this door if no door id was entered.

                Debug.LogWarning($"[Door][{name}]: Door ID is null or empty.");
            }

            if (Colliders == null || Colliders.Length <= 0)
            {
                Debug.LogWarning($"[Door][{name}]: This door has no colliders.");
            }

            _animator = GetComponent<Animator>();
            path = GetComponent<NpcPathSystem>();
        }

        private void Update()
        {
            if (!_IsOpen)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    if (_playerOnTrigger)
                        Open();
            }
            else
            {
                if (!OpenOnce)
                {
                    // Close the door after player gets away if this door doesnt load another scene.
                    if (_WasToggled)
                        return;

                    if (DistanceFromPlayer() > _distanceToClose)
                        CloseDoor();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                _playerOnTrigger = true;
                _WasToggled = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                _playerOnTrigger = false;
        }

        public override IEnumerator Toggle()
        {
            Debug.Log("Door toggled.");

            _WasToggled = true;

            if (!_IsOpen) 
                OpenDoor();
            else 
                CloseDoor();

            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        }

        public void Open()
        {
            if (IsLocked)
            {
                if (NeededItem != null)
                {
                    InventoryManager iManager = FindAnyObjectByType<InventoryManager>();
                    if (iManager == null)
                    {
                        Debug.LogError($"[Door][{name}]: No inventory manager was found on scene, this door will not work.");
                        return;
                    }

                    if (iManager.ConsumeItem(NeededItem.item_id, 1))
                    {
                        OpenDoor();
                        IsLocked = false;
                    }
                    else
                        ShowLockedDescription();
                }
                else
                    ShowLockedDescription();
            }
            else
                OpenDoor();
        }

        private void OpenDoor()
        {
            _animator.SetBool("Open", true);
            _IsOpen = true;
            IsLocked = false;

            StartCoroutine(LockPlayerMovement());

            foreach (BoxCollider col in Colliders)
                col.enabled = false;

            if (SceneToLoad != null)
                StartCoroutine(LoadScene());
        }

        private void ShowLockedDescription()
        {
            // Find a locked door dialogue.
            // For now, a generic dialogue will be used for every door.

            EventSequence dialogue = new EventSequence();

            SingleLine singleLine = new SingleLine("gate_locked", SingleLine.Type.World);
            dialogue.startEvent = singleLine;

            if (dialogue != null)
            { 
                EventManager eManager = FindAnyObjectByType<EventManager>();
                if(eManager != null && !eManager.busy)
                    eManager.StartSequence(dialogue, true);
            }
        }

        private void CloseDoor()
        {
            _animator.SetBool("Open", false);
            _IsOpen = false;

            foreach (BoxCollider col in Colliders)
                col.enabled = false;
        }

        private float DistanceFromPlayer()
        {
            Vector3 playerPosition = GameObject.FindAnyObjectByType<PlayerCore>().transform.position;
            float distance = Vector3.Distance(transform.position, playerPosition);

            return distance;
        }

        IEnumerator LockPlayerMovement()
        {
            // This will make the player unable to move for a moment while the door opens.

            PlayerCore player = FindAnyObjectByType<PlayerCore>();
            player.LockMovement();

            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

            player.UnlockMovement();
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
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

            // Make player move towards the door and wait.
            if (path)
            {
                yield return StartCoroutine(MovePlayerTowardsDoor());
            }

            // Start fade animation and wait.
            VFXManager vfxManager = FindAnyObjectByType<VFXManager>();
            yield return StartCoroutine(vfxManager.PlayVFX(VFXManager.VFX.Dark_FadeIn));

            // Load scene.
            SceneManager.LoadScene(SceneToLoad.name, LoadSceneMode.Single);
        }
    }
}
