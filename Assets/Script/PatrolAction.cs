
using UnityEngine;
using UnityEngine.AI;

public class PatrolAction : GOAPAction
{
    public Transform[] patrolPoints;

    public float patrolSpeed = 2f;

    private NavMeshAgent agent;
    private int currentPoint;

    private EnemyAnimationController animationController;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        animationController =
            GetComponentInChildren<EnemyAnimationController>();

        actionName = "Patrol";
        cost = 1f;

        // Patrol makes the enemy enter the patrolling state.
        Effects["IsPatrolling"] = true;
    }

    public override bool CheckProceduralCondition()
    {
        return agent != null &&
               patrolPoints != null &&
               patrolPoints.Length > 0;
    }

    public override void StartAction()
    {
        base.StartAction();

        agent.isStopped = false;

        // Clear any stopping distance left by a chase,
        // otherwise we stall short of the patrol points.
        agent.stoppingDistance = 0f;

        agent.speed = patrolSpeed;

        if (animationController != null)
        {
            animationController.PlayWalk();
        }
    }

    public override bool PerformAction()
    {
        if (agent == null ||
            patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            Status = ActionStatus.Failed;
            return false;
        }

        Status = ActionStatus.Running;

        agent.isStopped = false;

        Transform target =
            patrolPoints[currentPoint];

        if (target == null)
        {
            Status = ActionStatus.Failed;
            return false;
        }

        agent.SetDestination(target.position);

        if (!agent.pathPending &&
            agent.remainingDistance <= 0.5f)
        {
            currentPoint++;

            if (currentPoint >= patrolPoints.Length)
            {
                currentPoint = 0;
            }

            Debug.Log(
                "Patrol: Reached patrol point."
            );
        }

        return false;
    }

    public override void ResetAction()
    {
        base.ResetAction();

        if (agent != null)
        {
            agent.isStopped = true;
        }
    }
}

