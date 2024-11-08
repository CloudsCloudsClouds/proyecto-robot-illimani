namespace GlobalEnums
{
    public enum BEHAVIOR_STATES
    {
        IDLE,
        WANDER
    }

    public enum OBJECT_STATES
    {
        IDLE,
        MOVING,
        JUMPING,
        FLYING,
        FLINCH,
        FALLING,
        CLIMBING,
        HIT,
        ANIMATION,
        DEATH,
    }

    public enum TYPE_OF_UPDATE
    {
        PROCESS,
        PHYSICS,
    }

    public enum INPUT_ACTION
    {
        JUMP,
        INTERACT,
        GRAB,
        CLIMB,
    }

    public enum LIFECYCLE_STATE
    {
        JUST_ENTERED,
        ACTIVE,
        JUST_EXITED,
        NOT_ACTIVE
    }
}