using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void PlayWalk()
    {
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsRunning", false);
    }

    public void PlayRun()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", true);
    }

    public void StopMovementAnimation()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
    }

    public void PlayAttack()
    {
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWalking", true);

        animator.SetTrigger("attack");
    }

    public void PlayDeath()
    {
        animator.SetTrigger("dead");
    }
    public void PlayIdle()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
    }
}