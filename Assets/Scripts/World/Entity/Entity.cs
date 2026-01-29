using Assets.Scripts.Event_System;
using Assets.Scripts.Player;
using Assets.Scripts.World;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Core properties:
    public EntityAnimator entityAnimator;
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
    private float animSpeed;
    private float acceleration = 12f;
    private float deceleration = 18f;

    private Vector3 velocity;
    public Vector3 currentVelocity = Vector3.zero;
    private Vector3 moveVector = Vector3.zero;
    private PhysicsMaterial currentGroundMaterial;

    // Case specific properties:
    public bool onWater = false;
    public bool onDeepWater = false;
    public float currentWaterBodyHeight = 0f;


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
                moveSpeed = walkSpeed;
            else
                moveSpeed = runSpeed;

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
                    onDeepWater = false;
                }
                else if (playerDeepValue > 0.25f && playerDeepValue < waterDeepLimit)
                {
                    // Play heavy walk motion:
                    entityAnimator.animator.SetBool("Water/Enabled", true);
                    moveSpeed *= 0.5f;
                    onDeepWater = true;
                    Move();
                }
                else
                {
                    // Slow down while walking on water.
                    entityAnimator.animator.SetBool("Water/Enabled", false);
                    onDeepWater = false;
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

            // This prevents the player to rorate back to zero degrees.
            if (moveVector.magnitude > 0.001f)
            {
                // Rotate entity based on move vector:
                float playerRotationAngle = Mathf.Rad2Deg * Mathf.Atan2(moveVector.x, moveVector.z);
                Quaternion targetRotation = Quaternion.Euler(0, playerRotationAngle, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Apply acceleration:
            Vector3 targetVelocity = moveVector.sqrMagnitude > 0.001f ? moveVector.normalized * moveSpeed : Vector3.zero;
            float accel = moveVector.sqrMagnitude > 0.01f ? acceleration : deceleration;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, accel * Time.deltaTime);

            // Apply vertical movement:
            if (characterController.isGrounded && velocity.y < 0)
                velocity.y = -2f;
            velocity.y -= gravity * Time.deltaTime;

            Vector3 finalMove = currentVelocity;
            finalMove.y = velocity.y;
            characterController.Move(finalMove * Time.deltaTime);

            // Apply to animation:
            float targetAnimSpeed = currentVelocity.magnitude / moveSpeed;
            animSpeed = Mathf.Lerp(animSpeed, targetAnimSpeed, Time.deltaTime * 10f);
            entityAnimator.SetWalkBlendValue(animSpeed);
        }
    }

    public void MoveInWater()
    {
        // Swim motion.
        Debug.Log("Water movement");
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

            other.GetComponent<Water>().playerEntity = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
        {
            onWater = false;
            other.GetComponent<Water>().playerEntity = null;
        }
    }

    public bool isPlayer()
    {
        return gameObject.name == "Player";
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.point.y < transform.position.y + 0.075f)
        {
            currentGroundMaterial = hit.collider.material;
        }
    }

    public PhysicsMaterial GetGroundSurfaceMaterial()
    {
        if (!characterController.isGrounded)
            return null;

        return currentGroundMaterial;
    }
}

