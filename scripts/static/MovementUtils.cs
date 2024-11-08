using Godot;
using System;

public static class MovementUtils
{
    public static Vector3 Accelerate(Vector3 velocity, Vector3 wishDir, float maxVelocity, float maxAcceleration, double delta)
    {
        float currentSpeed = velocity.Dot(wishDir);
        float addSpeed = Mathf.Clamp(maxVelocity - currentSpeed, 0, maxAcceleration * (float)delta);
        return velocity + addSpeed * wishDir;
    }

    /// <summary>
    /// Updates the velocity of an entity on the ground.
    /// </summary>
    /// <param name="velocity">The current velocity of the entity.</param>
    /// <param name="wishDir">The desired direction of movement.</param>
    /// <param name="maxGroundSpeed">The maximum speed the entity can reach on the ground.</param>
    /// <param name="friction">The friction coefficient of the ground.</param>
    /// <param name="delta">The time elapsed since the last frame.</param>
    /// <returns>The new velocity of the entity.</returns>
    public static Vector3 UpdateVelocityGround(Vector3 velocity, Vector3 wishDir, float maxGroundSpeed, float friction, double delta)
    {
        float speed = velocity.Length();

        if (speed != 0)
        {
            float control = MathF.Max(1.5f, speed);
            float drop = control * friction * (float)delta;
            velocity *= Mathf.Max(speed - drop, 0) / speed;
        }

        return Accelerate(velocity, wishDir, maxGroundSpeed, maxGroundSpeed * 10, delta); 
    }

    /// <summary>
    /// Updates the velocity of an entity in the air.
    /// </summary>
    /// <param name="velocity">The current velocity of the entity.</param>
    /// <param name="wishDir">The desired direction of movement.</param>
    /// <param name="maxAirSpeed">The maximum speed the entity can reach in the air.</param>
    /// <param name="delta">The time elapsed since the last frame.</param>
    /// <returns>The new velocity of the entity.</returns>

    public static Vector3 UpdateVelocityAir(Vector3 velocity, Vector3 wishDir, float maxAirSpeed, double delta)
    {
        return Accelerate(velocity, wishDir, maxAirSpeed, maxAirSpeed * 10, delta); 
    }
}
