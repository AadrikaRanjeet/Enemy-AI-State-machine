//using System;
//using System.Collections.Generic;
//using UnityEngine;
//public class BeliefFactory
//{
//    readonly GoapAgent agent;
//    readonly Dictionary<string, AgentBelief> beliefs;
//    public BeliefFactory(GoapAgent agent,Dictionary<string, AgentBelief> beliefs)
//    {
//        this.agent = agent;
//        this.beliefs = beliefs;
//    }
//    public void AddBelief(string key, Func<bool> condition)
//    {
//      beliefs.Add(key, new AgentBelief.Builder(key).WithCondition(condition).Build());
//    }
//    ///  TODO add sensor Beliefs
    




//    public void AddLocationBelief(string key,float distance,Transform locationCondition)
//    {
//        AddLocationBelief(key, distance, locationCondition.position);
//    }
//    public void AddLocationBelief(string key,float distance,Vector3 locationCondition)
//    {
//        beliefs.Add(key,new AgentBelief.Builder(key)
//            .WithCondition(() => InRangeOf(locationCondition, distance))
//            .WithLocation(() => locationCondition)
//            .Build());

//    }
//    bool InRangeOf(Vector3 pos, float range)=>Vector3.Distance(agent.transform.position, pos) < range;
//}
//public class AgentBelief
//{
//    public string Name { get; }

//    Func<bool> condition = () => false; //all th beliefs can evaluate as true or false
//    Func<Vector3> observedLocation = () => Vector3.zero; //lot of agent's beliefs are about locations of things in the world, so we can have a function that returns a location
//    public Vector3 Location => observedLocation();



//    AgentBelief(string name)
//    {
//        Name = name;
//    }
//    public bool Evaluate()
//    {
//        return condition();
//    }

//    public class Builder
//    {
//        readonly AgentBelief belief;
//        public Builder(string name)
//        {
//            belief = new AgentBelief(name);
//        }

//        public Builder WithCondition(Func<bool> condition)
//        {
//            belief.condition = condition;
//            return this;
//        }
//        public Builder WithLocation(Func<Vector3> location)
//        {
//            belief.observedLocation = location;
//            return this;
//        }
//        public AgentBelief Build()
//        {
//            return belief;
//        }


//    }
//}
