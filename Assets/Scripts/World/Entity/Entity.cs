using Assets.Scripts.Event_System;
using Assets.Scripts.Player;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Core properties:
    private EntityAnimator entityAnimator;
    private CharacterController characterController;
    private PlayerInput playerInput;
    private EventManager eventManager;

    // General properties:
    public bool canMove = true;
    public bool isMoving = false;
    public bool isFollowingPath = false;

    public float gravity = 5;
    public float moveSpeed = 0f;
    public float walkSpeed = 0.5f;
    public float runSpeed = 1f;
    public float rotationSpeed = 10f;

    private Vector3 velocity;
    private Vector3 moveVector = Vector3.zero;

    // Case specific properties:
    private bool onWater = false;
    private float currentWaterBodyHeight = 0f;


    void Start()
    {
        entityAnimator = GetComponent<EntityAnimator>();
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        eventManager = FindAnyObjectByType<EventManager>();
    }

    private void Update()
    {
        if (name == "Player")
        {
            // Apply movement speed.
            if (Input.GetKey(KeyCode.LeftShift))
                moveSpeed = runSpeed;
            else
                moveSpeed = walkSpeed;

            if (onWater)
            {
                float playerYPos = transform.position.y;
                float waterDeepLimit = 0.5f;
                float playerDeepValue = currentWaterBodyHeight - playerYPos;

                // Do in-water motion and physics.
                if (playerDeepValue >= (waterDeepLimit))
                {
                    // Star swimming if it's too deep.
                    MoveInWater();
                }
                else if (playerDeepValue > 0.25f && playerDeepValue < waterDeepLimit)
                {
                    // Play heavy walk motion:
                    entityAnimator.animator.SetBool("Water/Enabled", true);
                    moveSpeed *= 0.5f;
                    Move();
                }
                else
                {
                    // Slow down while walking on water.
                    entityAnimator.animator.SetBool("Water/Enabled", false);
                    Move();
                }
            }
            else
            {
                // Do out-of-water motion and physics.

                // Move slower when in mid-air.
                if (!characterController.isGrounded)
                    moveSpeed *= 0.35f;

                // Move around.
                Move();
            }
        }
    }

    public void Move()
    {
        if (eventManager.busy || !GetComponent<PlayerCore>().canMove || playerInput.isGameMenuOpen())
            return;

        if (!isFollowingPath)
        {
            // Get move vector:
            moveVector = new Vector3(playerInput.GetHorizontalInput(), 0, playerInput.GetVerticalInput());

            // Apply animation:
            entityAnimator.SetWalkBlendValue(Mathf.Clamp01(moveVector.magnitude));

            // Ignore movement and rotation if no input is detected.
            if (moveVector.sqrMagnitude < 0.001f)
                return;

            // Rotate entity based on move vector:
            float playerRotationAngle = Mathf.Rad2Deg * Mathf.Atan2(moveVector.x, moveVector.z);
            Quaternion targetRotation = Quaternion.Euler(0, playerRotationAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Apply side movement:
            Vector3 horizontal = moveVector.normalized * moveSpeed;

            // Apply vertical movement:
            if(characterController.isGrounded && velocity.y < 0)
                velocity.y = -2f;
            velocity.y -= gravity * Time.deltaTime;

            // Combine both side and vertical movements:
            Vector3 finalMovement = horizontal + Vector3.up * velocity.y;

            // Apply movement:
            characterController.Move(finalMovement * Time.deltaTime);
        }
    }

    public void MoveInWater()
    { 
        // Swim motion.
    }

    public void LockMovement()
    {
        canMove = false;
        entityAnimator.StopWalking();
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            onWater = true;
            currentWaterBodyHeight = other.transform.position.y;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Water")
            onWater = false;
    }

    public bool isPlayer()
    {
        return gameObject.name == "Player";
    }
}

