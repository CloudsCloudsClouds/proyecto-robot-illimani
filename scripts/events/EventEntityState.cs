using Godot;

public class EventEntityState
{
    public Vector3 Velocity { set; get; }
    public Vector3 Position { set; get; }
    public Vector3 CollisionNormal { set; get; }
    public bool IsOnFloor { set; get; }

    public EventEntityState(Vector3 vel, Vector3 pos, Vector3 col_norm, bool is_on_floor)
    {
        Velocity = vel;
        Position = pos;
        CollisionNormal = col_norm;
        IsOnFloor = is_on_floor;
    }

    public EventEntityState()
    {
        Velocity = Vector3.Zero;
        Position = Vector3.Zero;
        CollisionNormal = Vector3.Zero;
        IsOnFloor = false;
    }
}