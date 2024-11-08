using Godot;
using System;

class EventDirection
{
    public Vector3 Direction { get; set; }
    public float Strenght { get; set; }

    public EventDirection(Vector3 dir, float strenght)
    {
        Direction = dir;
        
        if (!Direction.IsNormalized()) 
        {
            Direction = Direction.Normalized();
        }
        Strenght = strenght;
    }

    public Vector3 GetDirForce()
    {
        return Direction * Strenght;
    }
}