using System.Collections.Generic;

public class ObjectStateMachine
{
    private ObjectState _currentState;
    private List<ObjectTransition> _transitions = new List<ObjectTransition>();

    public void AddTransition(ObjectTransition transition)
    {
        _transitions.Add(transition);
    }

    public void SetInitialState(ObjectState state)
    {
        _currentState = state;
        _currentState.OnEnter?.Invoke();
    }

    public ObjectState GetCurrentState()
    {
        return _currentState;
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