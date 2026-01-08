using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    // ENUMERATOR FOR ANIMATION TYPES.
    public enum AnimationType
    { 
        MainPlayer,
        Enemy,
    }

    // ANIMATOR VARIABLES
    public Animator anim;
    public AnimationType animationType;

    public void setDirection(float dir)
    {
        anim.SetFloat("direction", dir);
    }

    public void setIsMoving(bool value)
    {
        if(animationType == AnimationType.MainPlayer)
            anim.SetBool("isMoving", value);
    }
}
