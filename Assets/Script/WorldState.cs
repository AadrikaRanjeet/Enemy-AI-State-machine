using UnityEngine;

using System.Collections.Generic;

public class WorldState
{
    private Dictionary<string, object> states = new Dictionary<string, object>();

    public void SetState(string key, object value)
    {
        states[key] = value;
    }

    public T GetState<T>(string key)
    {
        if (states.TryGetValue(key, out object value))
        {
            return (T)value;
        }

        return default;
    }

    public bool HasState(string key)
    {
        return states.ContainsKey(key);
    }

    public bool GetBool(string key)
    {
        return GetState<bool>(key);
    }
    public WorldState Clone()
    {
        WorldState copy = new WorldState();

        foreach (var state in states)
        {
            copy.SetState(state.Key, state.Value);
        }

        return copy;
    }
    public string GetStateKey()
    {
        List<string> keys =
            new List<string>();

        foreach (var state in states)
        {
            keys.Add(
                state.Key + "=" + state.Value
            );
        }

        keys.Sort();

        return string.Join("|", keys);
    }
}