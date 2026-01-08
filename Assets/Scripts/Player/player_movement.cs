using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour
{
    // ANIMATION VARIABLES
    public PlayerAnimations anim;

    // STEP VARIABLES
    public float stepDistance = 0.48f;
    public float stepSpeed = 0.24f;
    public float stepSmoothness = 0.1f;

    // VECTOR VARIABLES
    Vector3 velocity = Vector3.zero;
    Vector3 targetPosition = Vector3.zero;
    float distanceFromTargetPosition = 0.48f;

    // LOGIC VARIABLES
    bool isMoving = false;

    // PHYSICS VARIABLES
    CollisionMatrix matrix;
    public Vector2Int matrixPosition;

    void Start()
    {
        matrix = GameObject.Find("CollisionMatrix").GetComponent<CollisionMatrix>();

        if(!matrix)
        {
            Application.Quit();
        }

        targetPosition = transform.position;
    }

    void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                anim.setDirection(0);
                anim.setIsMoving(true);
                if(matrix.getCollisionType(matrixPosition.x, matrixPosition.y + 1) == 0)
                    StartCoroutine(movePlayer(0));
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                anim.setDirection(1);
                anim.setIsMoving(true);
                if (matrix.getCollisionType(matrixPosition.x, matrixPosition.y - 1) == 0)
                    StartCoroutine(movePlayer(1));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                anim.setDirection(2);
                anim.setIsMoving(true);
                if (matrix.getCollisionType(matrixPosition.x - 1, matrixPosition.y) == 0)
                    StartCoroutine(movePlayer(2));
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                anim.setDirection(3);
                anim.setIsMoving(true);
                if (matrix.getCollisionType(matrixPosition.x + 1, matrixPosition.y) == 0)
                    StartCoroutine(movePlayer(3));
            }
            else
            {
                anim.setIsMoving(false);
            }
        }
    }

    // MOVES PLAYER ONE STEP ON THE DESIRED DIRECTION:
    IEnumerator movePlayer(int dir)
    {
        // 0 = UP
        // 1 = DOWN
        // 2 = LEFT
        // 3 = RIGHT

        isMoving = true;

        switch (dir)
        {
            case 0:
                // MOVES UP
                targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + stepDistance);
                break;

            case 1:
                // MOVES DOWN
                targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - stepDistance);
                break;

            case 2:
                // MOVES LEFT
                targetPosition = new Vector3(transform.position.x - stepDistance, transform.position.y, transform.position.z);
                break;

            case 3:
                // MOVES RIGHT
                targetPosition = new Vector3(transform.position.x + stepDistance, transform.position.y, transform.position.z);
                break;

            default:
                break;
        }

        distanceFromTargetPosition = Vector3.Distance(transform.position, targetPosition);
        while (distanceFromTargetPosition != 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, stepSmoothness * stepSpeed);
            distanceFromTargetPosition = Vector3.Distance(transform.position, targetPosition);
            yield return null;
        }
        isMoving = false;
        targetPosition = Vector3.zero;

        switch (dir)
        {
            case 0:
                // MOVES UP
                matrixPosition.y += 1;
                break;

            case 1:
                // MOVES DOWN
                matrixPosition.y -= 1;
                break;

            case 2:
                // MOVES LEFT
                matrixPosition.x -= 1;
                break;

            case 3:
                // MOVES RIGHT
                matrixPosition.x += 1;
                break;

            default:
                break;
        }
    }   
}
