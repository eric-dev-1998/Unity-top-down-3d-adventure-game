using System.Collections;
using UnityEngine;
using System;

public class Entity : MonoBehaviour
{
    // PLAYER'S ANIMATION MANAGER CLASS
    [NonSerialized]
    public EntityAnimator animator;

    // STEP VARIABLES
    public float gravity = 5;
    public float stepDistance = 1f;
    public float moveSpeed = 0f;
    public float walkSpeed = 0.5f;
    public float runSpeed = 1f;
    public float rotationSpeed = 10f;
    public float stepSmoothness = 0.05f;
    public float slopeAngle = 0f;
    public float slopeAngleX = 0f;
    public float maxSlopeAngle = 45;

    // VECTOR VARIABLES
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Vector3 velocity;
    private Vector3 moveVector = Vector3.zero;

    public RaycastHit slopeHit;

    // LOGIC VARIABLES
    public bool isMoving = false;
    public bool isFollowingPath = false;

    // PHYSICS VARIABLES
    public EntityCollision collision;

    void Start()
    {
        animator = GetComponent<EntityAnimator>();
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        collision = GetComponent<EntityCollision>();
        //targetPosition = transform.position;
    }

    // THE FOLLOWING METHODS ARE FOR WALK MOTION
    // THE 'STEPS' PARAMETERS MEANS HOW MANY STEPS
    // THE ENTITY'S GOING TO MOVE.

    // THE FOLLOWING METHODS ALSO ARE FOR WALK MOTION
    // BUT RETURN A BOOLEAN VALUE INSTEAD, THESE CAN
    // BE USED TO TEST MOVEMENT ON IA.

    // THE FOLLOWING FUNCTION IS THE WALK MOTION MAIN
    // LOGIC.

    public bool isPlayer()
    {
        if (gameObject.name == "Player")
            return true;
        else
            return false;
    }

    public void MoveOnGround()
    {
        if (!isFollowingPath)
        {
            // Get move vector:
            moveVector = new Vector3(playerInput.GetHorizontalInput(), 0, playerInput.GetVerticalInput());

            // Apply animation:
            animator.SetWalkBlendValue(Mathf.Clamp01(moveVector.magnitude));

            // Ignore movement and rotation if no input is detected.
            if (moveVector.sqrMagnitude < 0.001f)
                return;

            // Rotate entity based on move vector:
            float playerRotationAngle = Mathf.Rad2Deg * Mathf.Atan2(moveVector.x, moveVector.z);
            Quaternion targetRotation = Quaternion.Euler(0, playerRotationAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Apply movement speed.
            if (Input.GetKey(KeyCode.LeftShift))
                moveSpeed = runSpeed;
            else
                moveSpeed = walkSpeed;

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
}

