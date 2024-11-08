using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;

public class ObjectStateMachineBuilder
{
    private ObjectStateMachine _stateMachine = new ObjectStateMachine();
    private List<ObjectState> _states = new List<ObjectState>();

    public ObjectStateMachineBuilder AddState(OBJECT_STATES name, Action onEnter = null, Action onUpdate = null, Action onExit = null)
    {
        var state = new ObjectState(name, onEnter, onUpdate, onExit);
        _states.Add(state);
        return this;
    }

    public ObjectStateMachineBuilder AddState(ObjectState state)
    {
        _states.Add(state);
        return this;
    }

    public ObjectStateMachineBuilder AddTransition(OBJECT_STATES fromStateName, OBJECT_STATES toStateName, Func<bool> condition)
    {
        var fromState = _states.FirstOrDefault(s => s.Name == fromStateName);
        var toState = _states.FirstOrDefault(s => s.Name == toStateName);

        if (fromState != null && toState != null)
        {
            _stateMachine.AddTransition(new ObjectTransition(fromState, toState, condition));
        }
        return this;
    }

    public ObjectStateMachineBuilder AddTransition(ObjectState fromStateState, ObjectState toStateState, Func<bool> condition)
    {
        var fromState = _states.FirstOrDefault(s => s.Equals(fromStateState));
        var toState = _states.FirstOrDefault(s => s.Equals(toStateState));

        if (fromState != null && toState != null)
        {
            _stateMachine.AddTransition(new ObjectTransition(fromState, toState, condition));
        }

        return this;
    }

    public ObjectStateMachine Build(OBJECT_STATES initialStateName)
    {
        var initialState = _states.FirstOrDefault(s => s.Name == initialStateName);
        if (initialState != null)
        {
            _stateMachine.SetInitialState(initialState);
        }
        return _stateMachine;
    }
}
