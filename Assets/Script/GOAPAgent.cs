using System.Collections.Generic;
using UnityEngine;

public class GOAPAgent : MonoBehaviour
{
    private WorldState worldState;

    private Dictionary<string, Belief> beliefs =
        new Dictionary<string, Belief>();

    private GOAPAction[] actions;
    public Vector3 lastKnownPlayerPosition;
    public bool hasLastKnownPosition;
    private GOAPPlanner planner;

    public Transform player;
    private PlayerSensor playerSensor;
    public float detectionRange = 10f;
    
    private AttackAction attackAction;
    private List<GOAPAction> currentPlan;
    private int currentActionIndex;
    private List<GOAPGoal> goals;
    private void Awake()
    {
        worldState = new WorldState();
        playerSensor = GetComponent<PlayerSensor>();
        attackAction = GetComponent<AttackAction>();
        CreateBeliefs();

        actions = GetComponents<GOAPAction>();

        planner = new GOAPPlanner();
        CreateGoals();
    }

    private void Start()
    {
        // Initial world state
        worldState.SetState("HasTarget", player != null);
        worldState.SetState("PlayerDetected", false);
        worldState.SetState("PlayerVisible", false);
        worldState.SetState("PlayerInRange", false);
        worldState.SetState("HasWeapon", true);
        worldState.SetState("LowHealth", false);
        worldState.SetState("PlayerDead", false);
        worldState.SetState("IsPatrolling", false);
        worldState.SetState("HasLastKnownPosition", false);
        // Give the actions their target
        foreach (GOAPAction action in actions)
        {
            if (action is MoveToPlayerAction moveAction)
            {
                moveAction.Setup(player);
            }

            if (action is AttackAction attackAction)
            {
                attackAction.Setup(player);
            }
        }
        worldState.SetState("HasLastKnownPosition", false);
        //   CreatePlan();
    }

    private void CreateBeliefs()
    {
        beliefs.Add(
        "PlayerDetected",
        new Belief(
            "PlayerDetected",
            () => playerSensor.PlayerDetected
        )
    );
        beliefs.Add(
            "PlayerInRange",
            new Belief(
                "PlayerInRange",
                () => IsPlayerInRange()
            )
        );
    }

    //private bool IsPlayerInRange()
    //{
    //    if (player == null)
    //    {
    //        return false;
    //    }

    //    float distance = Vector3.Distance(
    //        transform.position,
    //        player.position
    //    );

    //    float range = attackRange;

    //    if (attackAction != null)
    //    {
    //        range = attackAction.attackRange;
    //    }

    //    return distance <= range;
    //}
    private bool IsPlayerInRange()
    {
        if (player == null || attackAction == null)
        {
            return false;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        return distance <= attackAction.attackRange;
    }
    private void Update()
    {
        UpdateBeliefs();
        CheckForGoalChange();
        ExecutePlan();
    }
    private void CheckForGoalChange()
    {
        if (currentPlan == null)
        {
            return;
        }

        // Make sure the index is valid
        if (currentActionIndex < 0 ||
            currentActionIndex >= currentPlan.Count)
        {
            return;
        }

        bool playerDetected =
            worldState.GetBool("PlayerDetected");

        bool hasLastKnownPosition =
            worldState.GetBool("HasLastKnownPosition");

        GOAPAction currentAction =
            currentPlan[currentActionIndex];

        if (playerDetected)
        {
            // If player is detected while patrolling or investigating,
            // abandon the current plan and create a new one.
            if (currentAction.actionName == "Patrol" ||
                currentAction.actionName == "Investigate Last Known Position")
            {
                Debug.Log(
                    "GOAP: Player detected! Replanning..."
                );

                Replan();
            }

            return;
        }

        // Player was lost (behind a wall or out of range).
        // Stop chasing the live position and go to the
        // last known position instead.
        if (hasLastKnownPosition)
        {
            if (currentAction.actionName == "Move To Player" ||
                currentAction.actionName == "Attack Player")
            {
                Debug.Log(
                    "GOAP: Player lost! Investigating last known position..."
                );

                Replan();
            }
        }
    }
    private void UpdateBeliefs()
    {
        foreach (Belief belief in beliefs.Values)
        {
            bool value = belief.Evaluate();

            worldState.SetState(
                belief.Key,
                value
            );
        }
        if (playerSensor.PlayerDetected)
        {
            lastKnownPlayerPosition = player.position;
            hasLastKnownPosition = true;

            worldState.SetState(
                "HasLastKnownPosition",
                true
            );
        }
    }

    private void CreatePlan()
    {
        GOAPGoal selectedGoal = ChooseGoal();

        if (selectedGoal == null)
        {
            Debug.Log("GOAP: No suitable goal found.");
            return;
        }

        List<GOAPAction> actionList =
            new List<GOAPAction>(actions);

        currentPlan =
            planner.CreatePlan(
                worldState,
                actionList,
                selectedGoal
            );

        if (currentPlan == null)
        {
            Debug.Log(
                "GOAP: No plan found for " +
                selectedGoal.goalName
            );

            return;
        }

        Debug.Log(
            "GOAP: Planning for → " +
            selectedGoal.goalName
        );

        Debug.Log("GOAP PLAN CREATED:");

        foreach (GOAPAction action in currentPlan)
        {
            Debug.Log("→ " + action.actionName);
        }

        currentActionIndex = 0;
    }
    private void ExecutePlan()
    {
        if (currentPlan == null)
        {
            CreatePlan();
            return;
        }

        if (currentActionIndex >= currentPlan.Count)
        {
            Debug.Log("GOAP: Plan completed.");

            // Reset all actions
            foreach (GOAPAction action in actions)
            {
                action.ResetAction();
            }

            currentPlan = null;

            return;
        }

        GOAPAction currentAction =
            currentPlan[currentActionIndex];

        if (currentAction.Status == ActionStatus.NotStarted)
        {
            currentAction.StartAction();

            Debug.Log(
                "GOAP: Starting " +
                currentAction.actionName
            );
        }

        currentAction.PerformAction();

        if (currentAction.Status == ActionStatus.Completed)
        {
            Debug.Log(
                "GOAP: Completed " +
                currentAction.actionName
            );

            currentActionIndex++;
        }

        if (currentAction.Status == ActionStatus.Failed)
        {
            Debug.Log(
                "GOAP: Action failed. Replanning..."
            );

            Replan();
        }
    }
    private void Replan()
    {
        // Reset all actions
        foreach (GOAPAction action in actions)
        {
            action.ResetAction();
        }

        // Create a new plan
        CreatePlan();
    }
    private void CreateGoals()
    {
        goals = new List<GOAPGoal>();

        GOAPGoal attackGoal = new GOAPGoal(
            "Attack Player",
            new Dictionary<string, bool>
            {
            { "PlayerDead", true }
            },
            10
        );

        GOAPGoal idleGoal = new GOAPGoal(
            "Idle",
            new Dictionary<string, bool>
            {
            { "PlayerDetected", false }
            },
            1
        );
        GOAPGoal investigateGoal = new GOAPGoal(
            "Investigate",
            new Dictionary<string, bool>
            {
                { "HasLastKnownPosition", false }
            },
            5
        );
        GOAPGoal patrolGoal = new GOAPGoal(
            "Patrol",
            new Dictionary<string, bool>
            {
                { "IsPatrolling", true }
            },
            1
        );

        goals.Add(patrolGoal);
        goals.Add(attackGoal);
        goals.Add(idleGoal);
        goals.Add(investigateGoal);
    }
    private GOAPGoal ChooseGoal()
    {
        bool playerDetected =
            worldState.GetBool("PlayerDetected");

        bool hasLastKnownPosition =
            worldState.GetBool("HasLastKnownPosition");

        Debug.Log(
            "PlayerDetected = " +
            playerDetected
        );

        Debug.Log(
            "HasLastKnownPosition = " +
            hasLastKnownPosition
        );

        GOAPGoal bestGoal = null;

        foreach (GOAPGoal goal in goals)
        {
            Debug.Log(
                "Checking Goal: " +
                goal.goalName
            );

            // Attack only when player is detected
            if (goal.goalName == "Attack Player" &&
                !playerDetected)
            {
                Debug.Log(
                    "Attack skipped: Player not detected"
                );

                continue;
            }

            // Investigate only when we remember
            // the player's last position
            if (goal.goalName == "Investigate" &&
                !hasLastKnownPosition)
            {
                Debug.Log(
                    "Investigate skipped: No last known position"
                );

                continue;
            }

            // Don't choose a goal that is already achieved
            if (goal.IsAchieved(worldState))
            {
                Debug.Log(
                    goal.goalName +
                    " is already achieved"
                );

                continue;
            }

            // Choose the highest-priority suitable goal
            if (bestGoal == null ||
                goal.priority > bestGoal.priority)
            {
                bestGoal = goal;
            }
        }

        if (bestGoal != null)
        {
            Debug.Log(
                "Selected Goal: " +
                bestGoal.goalName
            );
        }
        else
        {
            Debug.Log(
                "No suitable GOAP goal found."
            );
        }

        return bestGoal;
    }
    public void ClearLastKnownPosition()
    {
        hasLastKnownPosition = false;

        worldState.SetState(
            "HasLastKnownPosition",
            false
        );
    }
}