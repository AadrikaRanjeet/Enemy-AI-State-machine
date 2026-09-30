using System.Collections.Generic;
using UnityEngine;
public enum ActionStatus
{
    NotStarted,
    Running,
    Completed,
    Failed
}
public abstract class GOAPAction : MonoBehaviour
{
    public ActionStatus Status { get; protected set; }
    public string actionName;
    public float cost = 1f;

    public Dictionary<string, bool> Preconditions =
        new Dictionary<string, bool>();

    public Dictionary<string, bool> Effects =
        new Dictionary<string, bool>();

    // Actually performs the action in the game
    public abstract bool PerformAction();

    // Tells the planner whether this action can currently be used
    public virtual bool CheckProceduralCondition()
    {
        return true;
    }

    // Check if the current world satisfies our preconditions
    public bool CanRun(WorldState worldState)
    {
        foreach (var condition in Preconditions)
        {
            if (!worldState.HasState(condition.Key))
            {
                return false;
            }

            if (worldState.GetBool(condition.Key) != condition.Value)
            {
                return false;
            }
        }

        return CheckProceduralCondition();
    }
    public void ApplyEffects(WorldState worldState)
    {
        foreach (var effect in Effects)
        {
            worldState.SetState(effect.Key, effect.Value);
        }
    }
    public virtual void StartAction()
    {
        Status = ActionStatus.Running;
       
    }
    public virtual void ResetAction()
    {
        Status = ActionStatus.NotStarted;
    }
}