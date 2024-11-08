using Godot;

public class EventApplyNewForce
{
    public Vector3 Direction { get; set; }
    public float Strenght { get; set; }
    public bool IsJumpForce { get; set; }

    public EventApplyNewForce(Vector3 dir, float strenght, bool is_jump_force)
    {
        Direction = dir;
        Strenght = strenght;
        IsJumpForce = is_jump_force;
    }
}