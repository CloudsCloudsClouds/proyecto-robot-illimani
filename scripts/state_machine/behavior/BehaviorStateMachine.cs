using System.Collections.Generic;

public class BehaviorStateMachine
{
    private BehaviorState _currentState;
    private List<BehaviorTransition> _transitions = new List<BehaviorTransition>();

    public void AddTransition(BehaviorTransition transition)
    {
        _transitions.Add(transition);
    }

    public void SetInitialState(BehaviorState state)
    {
        _currentState = state;
        _currentState.OnEnter?.Invoke();
    }

    public void Update()
    {
        _currentState.OnUpdate.Invoke();
        foreach (var transition in _transitions)
        {
            if (transition.From == _currentState && transition.Condition())
            {
                _currentState.OnExit?.Invoke();
                _currentState = transition.To;
                _currentState.OnEnter?.Invoke();
                break;
            }
        }
    }
}