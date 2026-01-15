using UnityEngine;

public class ZombieAnimatorManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator animator;
    void Start()
    {
        if(animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on ZombieAI.");
            }
        }
    }
    public void SetAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void SetDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

}
