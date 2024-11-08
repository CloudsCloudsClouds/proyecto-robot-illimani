using System;

public class ObjectTransition
{
    public ObjectState From { get; }
    public ObjectState To { get; }
    public Func<bool> Condition { get; }

    public ObjectTransition(ObjectState from, ObjectState to, Func<bool> condition)
    {
        From = from;
        To = to;
        Condition = condition;
    }
}