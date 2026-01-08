using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    public string id = "";
    public int currentSpriteIndex = 0;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWalkBlendValue(float value)
    {
        animator.SetFloat("MoveInput", value);
    }

    public Animator GetAnimator()
    { 
        return animator;
    }

    public float GetCurrentSpeed()
    {
        return animator.speed;
    }

    public void SetDirection(float value)
    {
        if (animator)
            animator.SetFloat("Direction", value);
    }

    public void SetWalkState(bool isWalking)
    {
        if (animator)
            animator.SetBool("Walk", isWalking);
    }

    public float GetDirection()
    {
        return animator.GetFloat("Direction");
    }
}
