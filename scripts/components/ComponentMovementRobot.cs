/************************************************************************************
 * Copyright (c) 2024  All Rights Reserved.                                         *
 *                                                                                  *
 *                                                                                  *
 * Daniel Bernal                                                                    *
 * like, good code man                                                              *
 *                                                                                  *
 ************************************************************************************/

using GlobalEnums;
using Godot;
using System;
using System.Diagnostics;
using static MovementUtils;


/// <summary>
/// Component that handles the movement of the robot. Uses a state machine to manage different movement states.
/// </summary>
[GlobalClass]
public partial class ComponentMovementRobot : IComponent
{
    /// <summary>
    /// Gets or sets the entity this component belongs to.
    /// </summary>
    public override WorldEntity Entity { get; set; }

    /// <summary>
    /// Gets or sets the type of update for this component.
    /// </summary>
    [Export]
    public override TYPE_OF_UPDATE TypeOfUpdate { get; set; } = TYPE_OF_UPDATE.PHYSICS;

    /// <summary>
    /// Gets or sets whether debug mode is enabled.
    /// </summary>
    [Export]
    public bool debug = false;

    /// <summary>
    /// Movement parameters.
    /// </summary>
    [ExportGroup("Movement")]
    private double JumpBufferCurrent = 0;
    [Export]
    private double JumpBufferUpLimit = 0.2;

    private double CoyoteBufferCurrent = 0;
    [Export]
    private double CoyoteBufferUpLimit = 0.1;

    [Export]
    private float MAX_GROUND_SPEED = 15;
    [Export]
    private float MAX_AIR_SPEED = 2;
    [Export]
    private float JUMP_TIME = 0.3f;
    [Export]
    private float JUMP_HEIGHT = 1.5f;
    [Export]
    private float FRICTION = 10f;

    /// <summary>
    /// Other parameters.
    /// </summary>
    private float JumpForce;
    private float Gravity;
    private Vector3 WishDir = Vector3.Zero;
    private double delta = 0;
    private bool IsRequestingJump;

    /// <summary>
    /// State machine for managing robot movement states.
    /// </summary>
    private ObjectStateMachine MyStateMachine;

    /// <summary>
    /// Initialization method. Sets the process mode and subscribes to the direction input event.
    /// </summary>
    public override void Init()
    {
        SetPhysicsProcess(false);
        SetProcessMode(ProcessModeEnum.Disabled);

		JumpForce = 2 * JUMP_HEIGHT / JUMP_TIME;
		Gravity = 2 * JUMP_HEIGHT / MathF.Pow(JUMP_TIME, 2);

        MyStateMachine = new ObjectStateMachineBuilder()
            .AddState(OBJECT_STATES.IDLE, state_IdleIntro, state_IddleUpdate, null)
            .AddState(OBJECT_STATES.MOVING, state_MovingIntro, state_MovingUpdate, null)
            .AddState(OBJECT_STATES.JUMPING, state_JumpIntro, state_JumpUpdate, null)
            .AddState(OBJECT_STATES.FALLING, state_FallIntro, state_FallUpdate, null)
            .AddTransition(OBJECT_STATES.IDLE, OBJECT_STATES.FALLING, cond_StartsFalling)
            .AddTransition(OBJECT_STATES.IDLE, OBJECT_STATES.MOVING, cond_IsMoving)
            .AddTransition(OBJECT_STATES.MOVING, OBJECT_STATES.IDLE, cond_IsStopping)
            .AddTransition(OBJECT_STATES.IDLE, OBJECT_STATES.JUMPING, cond_StartsJumping)
            .AddTransition(OBJECT_STATES.FALLING, OBJECT_STATES.MOVING, cond_HasLanded)
            .AddTransition(OBJECT_STATES.MOVING, OBJECT_STATES.FALLING, cond_StartsFalling)
            .AddTransition(OBJECT_STATES.MOVING, OBJECT_STATES.JUMPING, cond_StartsJumping)
            .AddTransition(OBJECT_STATES.JUMPING, OBJECT_STATES.FALLING, cond_StartsFalling)
            .AddTransition(OBJECT_STATES.FALLING, OBJECT_STATES.JUMPING, cond_StartsJumping)
            .Build(OBJECT_STATES.IDLE);
        
        Entity.EventBus.Subscribe<EventDirection>(event_DirectionInput);
        Entity.EventBus.Subscribe<EventInputAction>(event_InputAction);
    }

    //Update method
    //Updates the state machine and handles the delta time.
    public override void Update(double delta)
    {
        this.delta = delta;
        MyStateMachine.Update();
        if (debug)
        {
            GD.Print(Entity.Velocity, " ", MyStateMachine.GetCurrentState().Name, " ", WishDir);
        }
    }





    #region State Methods
    void state_IdleIntro()
    {
        GD.Print("Idle Intro");
    }

    void state_IddleUpdate()
    {
        var vel = Entity.Velocity;
        jump_CoyoteBufferReset();
        jump_JumpBufferAdvance(delta);
        vel = UpdateVelocityGround(vel, WishDir, MAX_GROUND_SPEED, FRICTION, delta);
        vel = force_ApplyGravity(vel, Vector3.Down, (float)delta, Gravity);
        vel = jump_TryJump(vel);
        Entity.Velocity = vel;
    }


    void state_MovingIntro()
    {
        GD.Print("Moving Intro");
    }

    void state_MovingUpdate()
    {
        var vel = Entity.Velocity;
        jump_JumpBufferAdvance(delta);
        jump_CoyoteBufferReset();
        vel = UpdateVelocityGround(vel, WishDir, MAX_GROUND_SPEED, FRICTION, delta);
        vel = force_ApplyGravity(vel, Vector3.Down, (float)delta, Gravity);
        vel = jump_TryJump(vel);
        Entity.Velocity = vel;
    }


    void state_FallIntro()
    {
        GD.Print("Fall Intro");
    }

    void state_FallUpdate()
    {
        var vel = Entity.Velocity;
        jump_CoyoteBufferStep(delta);
        jump_JumpBufferAdvance(delta);
        vel = UpdateVelocityAir(vel, WishDir, MAX_AIR_SPEED, delta);
        vel = force_ApplyGravity(vel, Vector3.Down, (float)delta, Gravity);
        vel = jump_TryJump(vel);
        Entity.Velocity = vel;
    }


    void state_JumpIntro()
    {
        GD.Print("Jump Intro");
    }

    void state_JumpUpdate()
    {
        var vel = Entity.Velocity;
        jump_CoyoteBufferStep(delta);
        jump_JumpBufferAdvance(delta);
        vel = UpdateVelocityAir(vel, WishDir, MAX_AIR_SPEED, delta);
        vel = force_ApplyGravity(vel, Vector3.Down, (float)delta, Gravity);
        vel = jump_TryJump(vel);
        Entity.Velocity = vel;
    }
    #endregion







    #region Conditional Methods
    //Conditional methods for state transitions
    //These methods determine whether a state transition should occur.
    bool cond_IsMoving()
    {
        return !WishDir.IsZeroApprox();
    }

    bool cond_IsStopping()
    {
        return WishDir.IsZeroApprox();
    }

    bool cond_StartsFalling()
    {
        return Entity.Velocity.Y < 0 && !Entity.IsOnFloor();
    }
    bool cond_StartsJumping()
    {
        return Entity.Velocity.Y > 0 && !Entity.IsOnFloor();
    }

    bool cond_HasLanded()
    {
        return Entity.IsOnFloor();
    }
    #endregion






    #region Event Methods
    //Event methods for handling input events
    //These methods handle events from the game's event bus.
    void event_DirectionInput(EventDirection @event)
    {
        WishDir = @event.Direction;
    }

    void event_InputAction(EventInputAction @event)
    {
        if (@event.State == LIFECYCLE_STATE.JUST_ENTERED)
        {
            jump_JumpBufferReset();
            IsRequestingJump = true;
        }
        if (@event.State == LIFECYCLE_STATE.JUST_EXITED || @event.State == LIFECYCLE_STATE.NOT_ACTIVE)
            IsRequestingJump = false;
    }
    #endregion







    #region Force Methods
    Vector3 force_ApplyGravity(Vector3 vel, Vector3 dir, float delta, float gravity)
    {
        var _vel = vel;
        _vel += dir * gravity * delta;
        return _vel;
    }

    Vector3 force_ApplyForce(Vector3 vel, Vector3 dir, float strengh)
    {
        var _vel = vel;
        _vel += dir * strengh;
        return _vel;
    }
    #endregion




    #region Jump Methods
    void jump_JumpBufferStep(double delta)
    {
        JumpBufferCurrent = Math.Min(JumpBufferCurrent + delta, JumpBufferUpLimit);
    }

    void jump_JumpBufferReset()
    {
        JumpBufferCurrent = 0;
    }


    void jump_CoyoteBufferStep(double delta)
    {
        CoyoteBufferCurrent = Math.Min(CoyoteBufferCurrent + delta, CoyoteBufferUpLimit);
    }

    void jump_CoyoteBufferReset()
    {
        CoyoteBufferCurrent = 0;
    }

    void jump_JumpBufferAdvance(double delta)
    {
        if (IsRequestingJump)
            jump_JumpBufferStep(delta);
        else 
        {
            JumpBufferCurrent = JumpBufferUpLimit;
        }
    }

    Vector3 jump_TryJump(Vector3 vel)
    {
        if (jump_IsJumpPossible())
        {
            jump_ConsumeJump();
            return force_ApplyForce(vel, Vector3.Up, JumpForce);
        }
        return vel;
    }

    bool jump_IsJumpPossible()
    {
        return JumpBufferCurrent < JumpBufferUpLimit && CoyoteBufferCurrent < CoyoteBufferUpLimit; 
    }

    void jump_ConsumeJump()
    {
        JumpBufferCurrent = JumpBufferUpLimit;
        CoyoteBufferCurrent = CoyoteBufferUpLimit;
    }
    #endregion
}
