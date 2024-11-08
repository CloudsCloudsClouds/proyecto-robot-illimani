using GlobalEnums;

class EventInputAction
{
    public INPUT_ACTION Action { get; set; }
    public LIFECYCLE_STATE State { get; set; }

    public EventInputAction(INPUT_ACTION _action, LIFECYCLE_STATE _state)
    {
        Action = _action;
        State = _state;
    }
}