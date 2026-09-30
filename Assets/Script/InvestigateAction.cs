
using UnityEngine;
using UnityEngine.AI;

public class InvestigateAction : GOAPAction
{
    private NavMeshAgent agent;

    private EnemyAnimationController animationController;

    public float investigateWaitTime = 2f;

    private float waitTimer;

    private GOAPAgent goapAgent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        animationController =
            GetComponentInChildren<EnemyAnimationController>();

        goapAgent =
            GetComponent<GOAPAgent>();

        actionName = "Investigate Last Known Position";
        cost = 2f;

        Preconditions["HasLastKnownPosition"] = true;

        Effects["HasLastKnownPosition"] = false;
    }

    public override bool CheckProceduralCondition()
    {
        return goapAgent != null &&
               goapAgent.hasLastKnownPosition;
    }

    public override void StartAction()
    {
        base.StartAction();

        waitTimer = 0f;

        agent.isStopped = false;

        // MoveToPlayerAction leaves a stopping distance on the
        // shared agent, which would stop us short of the target.
        agent.stoppingDistance = 0f;

        agent.SetDestination(
            goapAgent.lastKnownPlayerPosition
        );

        if (animationController != null)
        {
            animationController.PlayRun();
        }

        Debug.Log(
            "GOAP: Investigating last known player position."
        );
    }

    public override bool PerformAction()
    {
        if (goapAgent == null ||
            !goapAgent.hasLastKnownPosition)
        {
            Status = ActionStatus.Failed;
            return false;
        }

        if (agent.pathPending)
        {
            return false;
        }

        // Still travelling to the last known position.
        if (agent.remainingDistance > 0.5f)
        {
            if (animationController != null)
            {
                animationController.PlayRun();
            }

            return false;
        }

        // We reached the last known position.
        agent.isStopped = true;

        // Search / wait here.
        waitTimer += Time.deltaTime;

        if (animationController != null)
        {
            animationController.PlayIdle();
        }

        if (waitTimer >= investigateWaitTime)
        {
            goapAgent.ClearLastKnownPosition();

            Status = ActionStatus.Completed;

            Debug.Log(
                "GOAP: Investigation finished. Player not found."
            );

            return true;
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

        waitTimer = 0f;
    }
}

