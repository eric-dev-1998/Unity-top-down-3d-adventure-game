using Assets.Scripts.Player;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    // PLAYER'S ENTITY CLASS
    private Entity entity;

    // PLAYER'S INPUT MANAGER CLASS
    private PlayerInput playerInput;

    // Player audio manager:
    private PlayerAudio playerAudio;

    // Player properties:
    public bool canMove = true;
    public bool onWater = false;
    public float currentWaterHeight = 0f;

    private float velocity = 0;
    private float lookDistance = 2f;
    private float lookWeight = 0.45f;
    private float currentWeight = 0f;
    [HideInInspector]
    public Transform objectOnSight;
    private Vector3 smoothedTarget = Vector3.zero;

    // AWAKE METHOD, TRIGGERS BEFORE THE START METHOD.
    private void Awake()
    {
        // PLAYER ENTITY SOULD BE ASSINGED, IF NOT, WE LOOK
        // FOR IT.

        entity = GetComponent<Entity>();

        if(!entity)
        {
            entity = gameObject.GetComponent<Entity>();

            // IF THE PLAYER ENTITY DOESNT EXISTS, THE GAME
            // QUITS.
            if (!entity)
                Application.Quit();
        }

        // PLAYER INPUT MANAGER SHOULD BE ASSIGNED, IF NOT,
        // WE ASSIGN A NEW INSTANCE.

        playerInput = GetComponent<PlayerInput>();

        playerAudio = new PlayerAudio();

        if(!playerInput)
        {
            playerInput = gameObject.GetComponent<PlayerInput>();
            if (!playerInput)
            {
                gameObject.AddComponent<PlayerInput>();
                playerInput = gameObject.GetComponent<PlayerInput>();
            }
        }
    }

    private void Update()
    {
        // SIMPLE WALK LOGIC.
        if (!FindAnyObjectByType<Assets.Scripts.Event_System.EventManager>().busy)
        {
            if (canMove && !playerInput.isGameMenuOpen())
            {
                entity.MoveOnGround();
            }
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (objectOnSight)
        {
            float distance = Vector3.Distance(transform.position, objectOnSight.position);
            float targetWeight = distance < lookDistance ? lookWeight : 0f;
            float smoothSpeed = 5f;

            currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * smoothSpeed);

            Transform playerHead = objectOnSight.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).transform;

            if (smoothedTarget == Vector3.zero)
                smoothedTarget = playerHead.position;

            smoothedTarget = Vector3.Lerp(smoothedTarget, playerHead.position, Time.deltaTime * 5f);

            GetComponent<Animator>().SetLookAtWeight(currentWeight);
            GetComponent<Animator>().SetLookAtPosition(smoothedTarget);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            currentWaterHeight = other.ClosestPoint(transform.position).y;
            onWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
            onWater = false;
    }

    public Entity GetEntity()
    {
        return entity;
    }

    public PlayerAudio GetAudio()
    {
        return playerAudio;
    }

    public void LockMovement()
    {
        canMove = false;

        // Slowly reduce movement animation value to stop:
        float targetValue = Mathf.SmoothDamp(GetComponent<Animator>().GetFloat("MoveInput"), 0, ref velocity, Time.deltaTime * 0.03f);
        GetComponent<Animator>().SetFloat("MoveInput", targetValue);
    }

    public void UnlockMovement()
    {
        canMove = true;
    }
}
