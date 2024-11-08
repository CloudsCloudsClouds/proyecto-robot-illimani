using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;

public class BehaviorStateMachineBuilder
{
    private BehaviorStateMachine _stateMachine = new BehaviorStateMachine();
    private List<BehaviorState> _states = new List<BehaviorState>();

    public BehaviorStateMachineBuilder AddState(BEHAVIOR_STATES name, Action onEnter = null, Action onUpdate = null, Action onExit = null)
    {
        var state = new BehaviorState(name, onEnter, onUpdate, onExit);
        _states.Add(state);
        return this;
    }

    public BehaviorStateMachineBuilder AddState(BehaviorState state)
    {
        _states.Add(state);
        return this;
    }

    public BehaviorStateMachineBuilder AddTransition(BEHAVIOR_STATES fromStateName, BEHAVIOR_STATES toStateName, Func<bool> condition)
    {
        var fromState = _states.FirstOrDefault(s => s.Name == fromStateName);
        var toState = _states.FirstOrDefault(s => s.Name == toStateName);

        if (fromState != null && toState != null)
        {
            _stateMachine.AddTransition(new BehaviorTransition(fromState, toState, condition));
        }
        return this;
    }

    public BehaviorStateMachineBuilder AddTransition(BehaviorState fromStateState, BehaviorState toStateState, Func<bool> condition)
    {
        var fromState = _states.FirstOrDefault(s => s.Equals(fromStateState));
        var toState = _states.FirstOrDefault(s => s.Equals(toStateState));

        if (fromState != null && toState != null)
        {
            _stateMachine.AddTransition(new BehaviorTransition(fromState, toState, condition));
        }

        return this;
    }

    public BehaviorStateMachine Build(BEHAVIOR_STATES initialStateName)
    {
        var initialState = _states.FirstOrDefault(s => s.Name == initialStateName);
        if (initialState != null)
        {
            _stateMachine.SetInitialState(initialState);
        }
        return _stateMachine;
    }
}
