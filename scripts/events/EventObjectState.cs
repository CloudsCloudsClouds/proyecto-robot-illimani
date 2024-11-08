using GlobalEnums;

class EventObjectState
{
    public OBJECT_STATES State { set; get; }

    public LIFECYCLE_STATE  LifeCycleState { set; get; }

    public EventObjectState(OBJECT_STATES _state, LIFECYCLE_STATE _life_state)
    {
        State = _state;
        LifeCycleState = _life_state;
    }
}