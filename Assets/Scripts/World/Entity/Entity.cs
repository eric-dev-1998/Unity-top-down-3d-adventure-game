using System.Collections;
using UnityEngine;
using System;

public class Entity : MonoBehaviour
{
    // PLAYER'S ANIMATION MANAGER CLASS
    [NonSerialized]
    public EntityAnimator animator;

    // STEP VARIABLES
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
            Vector3 moveVector = new Vector3(playerInput.GetHorizontalInput(), 0, playerInput.GetVerticalInput());

            // Apply animation:
            animator.SetWalkBlendValue(Mathf.Clamp01(moveVector.magnitude));

            // Ignore movement and rotation if no input is detected.
            if (moveVector.sqrMagnitude < 0.0001f)
                return;

            // Apply movement speed.
            if (Input.GetKeyDown(KeyCode.LeftShift))
                moveSpeed = walkSpeed;
            else
                moveSpeed = runSpeed;

            // Rotate entity based on move vector:
            float playerRotationAngle = Mathf.Rad2Deg * Mathf.Atan2(moveVector.x, moveVector.z);
            Quaternion targetRotation = Quaternion.Euler(0, playerRotationAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Apply speed modifier to move vector to move entity around:
            moveVector *= moveSpeed * Time.deltaTime;

            // Apply movement:
            characterController.Move(moveVector);
        }
    }
}

