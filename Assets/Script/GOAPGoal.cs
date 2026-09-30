
using System.Collections.Generic;

public class GOAPGoal
{
    public string goalName;

    public Dictionary<string, bool> DesiredState =
        new Dictionary<string, bool>();

    public int priority;

    public GOAPGoal(
        string name,
        Dictionary<string, bool> desiredState,
        int priority)
    {
        goalName = name;
        DesiredState = desiredState;
        this.priority = priority;
    }

    public bool IsAchieved(WorldState worldState)
    {
        foreach (var condition in DesiredState)
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

        return true;
    }
}

