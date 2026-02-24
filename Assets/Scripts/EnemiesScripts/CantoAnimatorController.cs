using UnityEngine;

public class CantoAnimatorController : MonoBehaviour
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
        animator.SetBool("IsWalking", isWalking);
        animator.Update(0f);
    }


    public void SetHit()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    public void SetDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

    public void SetAttack(int attackType)
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack" + attackType);
        }
    }

    public void SetCastMagicAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("CastMagicAttack");
        }
    }

    public void ResetTriggers()
    {
        animator.ResetTrigger("Hit");
        animator.ResetTrigger("Death");
    }

}
