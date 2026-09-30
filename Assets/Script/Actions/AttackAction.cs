using UnityEngine;

public class AttackAction : GOAPAction
{
    private Transform player;

    public float attackCooldown = 1f;
    public float attackRange = 2f;
    private float nextAttackTime;
    private EnemyAnimationController animationController;
    private void Awake()
    {
        animationController =
    GetComponent<EnemyAnimationController>();
        actionName = "Attack Player";
        cost = 1f;

        Preconditions["PlayerInRange"] = true;

        Effects["PlayerDead"] = true;
    }

    public void Setup(Transform target)
    {
        player = target;
    }

    public override bool CheckProceduralCondition()
    {
        return player != null;
    }

    public override bool PerformAction()
    {
        if (player == null)
        {
            Status = ActionStatus.Failed;
            return false;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // Player moved out of attack range
        if (distance > attackRange)
        {
            Status = ActionStatus.Failed;

            Debug.Log("Attack failed: Player is too far away.");

            return false;
        }

        Status = ActionStatus.Running;

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            Debug.Log("ATTACKING PLAYER!");

            PlayPlayerDeathAnimation();

            Status = ActionStatus.Completed;

            return true;
        }

        return false;
    }

    private void PlayPlayerDeathAnimation()
    {
        if (player == null)
        {
            return;
        }

        Animator playerAnimator =
            player.GetComponentInChildren<Animator>();

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("dead");
        }
    }
    public override void StartAction()
    {
        base.StartAction();

        animationController.PlayAttack();
    }
}