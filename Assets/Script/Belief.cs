using System;

public class Belief
{
    public string Key { get; private set; }

    private Func<bool> condition;

    public Belief(string key, Func<bool> condition)
    {
        Key = key;
        this.condition = condition;
    }

    public bool Evaluate()
    {
        return condition();
    }
}