using System;
using GlobalEnums;

public class ObjectState
{
    public OBJECT_STATES Name { get; }
    public Action OnEnter { get; }
    public Action OnUpdate { get; }
    public Action OnExit { get; }

    public ObjectState(OBJECT_STATES name, Action onEnter, Action onUpdate, Action onExit)
    {
        Name = name;
        OnEnter = onEnter;
        OnUpdate = onUpdate;
        OnExit = onExit;
    }   
}