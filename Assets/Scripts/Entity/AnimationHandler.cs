using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMove"); //Hash라는 고유한 숫자값으로 변환
    private static readonly int IsDamage = Animator.StringToHash("isDamage");
    protected Animator animator;
    protected virtual void Awake()
    {   
        animator = GetComponentInChildren<Animator>();
    }

    public void Move(Vector2 obj) 
    {
        animator.SetBool(IsMoving, obj.magnitude > .5f);
    }

    public void Damage()
    {
        animator.SetBool(IsDamage, true);
    }

    public void InvincibilityEnd() 
    {
        animator.SetBool(IsDamage, false);
    }
}
