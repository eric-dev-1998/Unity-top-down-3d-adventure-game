using System;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    [NonSerialized]
    public Animator animator;

    private float lookDistance = 2f;
    private float lookWeight = 0.45f;
    private float currentWeight = 0f;
    private float velocity;

    [HideInInspector]
    public Transform objectOnSight;
    private Vector3 smoothedTarget = Vector3.zero;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (objectOnSight != null)
        {
            float distance = Vector3.Distance(transform.position, objectOnSight.position);
            float targetWeight = distance < lookDistance ? lookWeight : 0f;
            float smoothSpeed = 5f;

            currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * smoothSpeed);

            Transform playerHead = objectOnSight.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).transform;

            if (smoothedTarget == Vector3.zero)
                smoothedTarget = playerHead.position;

            smoothedTarget = Vector3.Lerp(smoothedTarget, playerHead.position, Time.deltaTime * 5f);

            float blend = animator.GetFloat("MoveInput");
            float runLookModifier = Mathf.Lerp(1.0f, 0.35f, blend);

            GetComponent<Animator>().SetLookAtWeight(currentWeight * runLookModifier);

            Vector3 target = smoothedTarget;
            if (blend > 0.6f)
                target.y = animator.GetBoneTransform(HumanBodyBones.Head).position.y;

            GetComponent<Animator>().SetLookAtPosition(target);
        }
    }

    public void SetWalkBlendValue(float value)
    {
        animator.SetFloat("MoveInput", value);
    }

    public void StopWalking()
    {
        float targetValue = Mathf.SmoothDamp(animator.GetFloat("MoveInput"), 0, ref velocity, Time.deltaTime * 0.03f);
        animator.SetFloat("MoveInput", targetValue);
    }
}
