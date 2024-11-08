using System;

public class BehaviorTransition
{
    public BehaviorState From { get; }
    public BehaviorState To { get; }
    public Func<bool> Condition { get; }

    public BehaviorTransition(BehaviorState from, BehaviorState to, Func<bool> condition)
    {
        From = from;
        To = to;
        Condition = condition;
    }
}