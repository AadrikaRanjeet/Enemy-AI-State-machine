using UnityEngine;
using UnityEngine.AI;

public class MoveToPlayerAction : GOAPAction
{
    private NavMeshAgent agent;
    private Transform player;

    public float stoppingDistance = 2f;
    public float chaseSpeed = 6.5f;
    private EnemyAnimationController animationController;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponentInChildren<EnemyAnimationController>();

        actionName = "Move To Player";
        cost = 2f;

        Preconditions["HasTarget"] = true;

        Effects["PlayerInRange"] = true;
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

        agent.isStopped = false;
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(player.position);

        Status = ActionStatus.Running;

        if (!agent.pathPending &&
            agent.remainingDistance <= stoppingDistance)
        {
            agent.isStopped = true;

            Status = ActionStatus.Completed;

            Debug.Log("MoveToPlayer completed!");

            return true;
        }

        return false;
    }
    public override void StartAction()
    {
        base.StartAction();

        agent.speed = chaseSpeed;

        animationController.PlayRun();

        
    }
}