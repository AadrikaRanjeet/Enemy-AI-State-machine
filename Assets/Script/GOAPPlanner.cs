using System.Collections.Generic;
using System;
public class GOAPPlanner
{
    public List<GOAPAction> CreatePlan(
        WorldState worldState,
        List<GOAPAction> actions,
        GOAPGoal goal)
    {
        Node startNode = new Node(
            null,
            null,
            worldState.Clone(),
            0
        );

        List<Node> leaves = new List<Node>();

        HashSet<string> visitedStates =
            new HashSet<string>();

        BuildGraph(
            startNode,
            leaves,
            actions,
            goal,
            visitedStates
        );

        if (leaves.Count == 0)
        {
            return null;
        }

        Node cheapest = null;
        float cheapestCost = float.MaxValue;

        foreach (Node leaf in leaves)
        {
            if (leaf.cost < cheapestCost)
            {
                cheapest = leaf;
                cheapestCost = leaf.cost;
            }
        }

        return BuildPlanFromNode(cheapest);
    }

    private void BuildGraph(
        Node parent,
        List<Node> leaves,
        List<GOAPAction> actions,
        GOAPGoal goal,
        HashSet<string> visitedStates)
    {
        string stateKey = CreateStateKey(parent.state);

        if (visitedStates.Contains(stateKey))
        {
            return;
        }

        visitedStates.Add(stateKey);

        foreach (GOAPAction action in actions)
        {
            if (!action.CanRun(parent.state))
            {
                continue;
            }

            WorldState newState =
                parent.state.Clone();

            action.ApplyEffects(newState);

            string newStateKey =
                CreateStateKey(newState);

            if (visitedStates.Contains(newStateKey))
            {
                continue;
            }

            Node newNode = new Node(
                parent,
                action,
                newState,
                parent.cost + action.cost
            );

            if (goal.IsAchieved(newState))
            {
                leaves.Add(newNode);
            }
            else
            {
                BuildGraph(
                    newNode,
                    leaves,
                    actions,
                    goal,
                    visitedStates
                );
            }
        }
    }

    private string CreateStateKey(WorldState state)
    {
        return state.GetStateKey();
    }

    private List<GOAPAction> BuildPlanFromNode(Node node)
    {
        List<GOAPAction> plan =
            new List<GOAPAction>();

        while (node != null && node.action != null)
        {
            plan.Insert(0, node.action);

            node = node.parent;
        }

        return plan;
    }

    private class Node
    {
        public Node parent;
        public GOAPAction action;
        public WorldState state;
        public float cost;

        public Node(
            Node parent,
            GOAPAction action,
            WorldState state,
            float cost)
        {
            this.parent = parent;
            this.action = action;
            this.state = state;
            this.cost = cost;
        }
    }
}