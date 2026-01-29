using UnityEngine;

public class CruzAI : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on CruzAI.");
        }
    }

    public void SetWalking(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
        }
    }

    public void SetPunch2()
    {
        if (animator != null)
        {
            animator.SetTrigger("Punch2");
        }
    }

    public void SetPunch3()
    {
        if (animator != null)
        {
            animator.SetTrigger("Punch3");
        }
    }

    public void SetHurt()
    {
        if (animator != null)
        {
            animator.SetTrigger("GetDamage");
        }
    }

    public void SetThrow()
    {
        if (animator != null)
        {
            animator.SetTrigger("Throw");
        }
    }

    public void SetDead()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }
    public void ResetAttackTriggers()
    {
        animator.ResetTrigger("Punch2");
        animator.ResetTrigger("Punch3");
        animator.ResetTrigger("Throw");
    }

}
