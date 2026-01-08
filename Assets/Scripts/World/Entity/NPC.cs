using System.Collections;
using UnityEngine;

using Assets.Scripts.Event_System;
using Assets.Scripts.Quest_System;
using Assets.Scripts.World.Npc;

[RequireComponent(typeof(NpcDialogue))]
public class NPC : MonoBehaviour
{
    // NPC Name:
    public string npc_id = "";

    private NpcDialogue dialogue;

    public EventSequence eventSequence;

    private Assets.Scripts.Quest_System.Manager manager;

    // NPC entity:
    // Used to acces the animator controller component and path system.
    Entity entity;
    Assets.Scripts.Event_System.Manager eventManager;
    Animator animator;

    // Current NPC state:
    public int state = 0;

    // Properties:
    public bool isLookingAtPlayer = false;
    public bool onEvent = false;

    private bool isPlayerOnTrigger = false;
    private float lookDistance = 2f;
    private float lookWeight = 0.45f;
    private float currentWeight = 0f;

    private Vector3 smoothedTarget = Vector3.zero;
    private Transform playerTransform;
    private Transform head;
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private Quaternion targetPlayerRotation;

    private void Start()
    {
        dialogue = GetComponent<NpcDialogue>();
        dialogue.Load(npc_id);

        entity = GetComponent<Entity>();

        eventManager = FindAnyObjectByType<Assets.Scripts.Event_System.Manager>();

        // Animation related properties.
        animator = GetComponent<Animator>();
        head = animator.GetBoneTransform(HumanBodyBones.Head);

        // Transform properties:
        originalRotation = transform.rotation;
        playerTransform = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if (!onEvent)
        {
            if (!entity.isFollowingPath)
            {
                // Turn npc back to its original position.
                transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * 2);
                if (Quaternion.Angle(transform.rotation, originalRotation) < 0.005f)
                    transform.rotation = originalRotation;
            }

            // Try trigger event if player is on trigger and action input is detected.
            if (isPlayerOnTrigger && Input.GetKeyDown(KeyCode.Space))
            {
                // Enter on event.
                if (!eventManager.busy)
                {
                    manager.InteractedWithNPC(npc_id);
                    StartCoroutine(StartEvent());
                }
            }
        }
        else
        {
            playerTransform.GetComponent<MainPlayer>().LockMovement();

            TurnToPlayer();
            TurnPlayer();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        float targetWeight = distance < lookDistance ? lookWeight : 0f;
        float smoothSpeed = 5f;

        if (targetWeight != 0)
            playerTransform.GetComponent<MainPlayer>().objectOnSight = this.transform;

        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * smoothSpeed);

        Transform playerHead = playerTransform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).transform;

        if (smoothedTarget == Vector3.zero)
            smoothedTarget = playerHead.position;

        smoothedTarget = Vector3.Lerp(smoothedTarget, playerHead.position, Time.deltaTime * 5f);

        animator.SetLookAtWeight(currentWeight);
        animator.SetLookAtPosition(smoothedTarget);
    }

    private void OnSequenceEnd()
    {
        onEvent = false;
        playerTransform.GetComponent<MainPlayer>().UnlockMovement();
    }

    private void TurnToPlayer()
    {
        // Turn npc to player.
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;
        targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);

        // Play animation.
        animator.SetBool("Turn/Enabled", true);
        if (targetRotation.eulerAngles.y > transform.eulerAngles.y)
            animator.SetBool("Turn/Left", false);
        else
            animator.SetBool("Turn/Left", true);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 10f)
        {
            transform.rotation = targetRotation;

            // Stop animation.
            animator.SetBool("Turn/Enabled", false);
            animator.SetBool("Turn/Left", false);
        }
    }

    private void TurnPlayer()
    {
        Animator playerAnimator = playerTransform.GetComponent<Animator>();

        // Turn player to npc.
        Vector3 direction = (transform.position - playerTransform.position).normalized;
        direction.y = 0;
        targetPlayerRotation = Quaternion.LookRotation(direction);
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetPlayerRotation, 4 * Time.deltaTime);

        // Play player turn animation.
        playerAnimator.SetBool("Turn/Enabled", true);
        if (targetRotation.eulerAngles.y > transform.eulerAngles.y)
            playerAnimator.SetBool("Turn/Left", false);
        else
            playerAnimator.SetBool("Turn/Left", true);

        if (Quaternion.Angle(playerTransform.rotation, targetPlayerRotation) < 0.5f)
        {
            playerTransform.rotation = targetPlayerRotation;

            // Stop turn animation.
            playerAnimator.SetBool("Turn/Enabled", false);
            playerAnimator.SetBool("Turn/Left", false);
        }
    }

    private IEnumerator StartEvent()
    {
        onEvent = true;
        Assets.Scripts.Event_System.Manager manager = FindAnyObjectByType<Assets.Scripts.Event_System.Manager>();

        if (manager == null)
            yield break;

        manager.OnEventFinished += OnSequenceEnd;

        yield return new WaitUntil(() => transform.rotation == targetRotation && playerTransform.rotation == targetPlayerRotation);
        manager.StartSequence(eventSequence);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            isPlayerOnTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            isPlayerOnTrigger = false;
    }
}