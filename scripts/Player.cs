using Godot;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

[GlobalClass]
public partial class Player : CharacterBody3D
{
	[Export]
	public Node3D Head;
	[Export]
	public bool HeadTilt = true;
	[Export]
	public bool weaponTilt = true;
	[Export]
	public Camera3D Camera;
	[Export]
	public Node3D Weapon;
	[Export]
	public float CameraSensitivity = 0.05f;


	//	---
	//	VARIABLES
	//	---
	//	---
	//		INPUT AND CONTROL VARIABLES
	//	---
	private double JumpBufferCurrent = 0;
	[Export]
	private double JumpBufferUpLimit = 0.2;

	private double CoyoteBufferCurrent = 0;
	[Export]
	private double CoyoteBufferUpLimit = 0.1;

	private Vector3 Direction = Vector3.Zero;

	//	---
	//		MOVEMENT AND SPEED VARIABLES
	//	---
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

	private float MAX_ACCELERATION = 10;
	private float JumpForce;
	private float Gravity;
	private Vector2 CameraVelocity = Vector2.Zero;

	// 	---
	//	FUNCTIONS
	// 	---
	//	---
	//		OVERRIDE FUNCTIONS
	//	---
	public override void _Ready()
	{
		MAX_ACCELERATION *= MAX_GROUND_SPEED;
		JumpForce = 2 * JUMP_HEIGHT / JUMP_TIME;
		Gravity = 2 * JUMP_HEIGHT / MathF.Pow(JUMP_TIME, 2);
		Input.SetMouseMode(Input.MouseModeEnum.Captured);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseModeEnum.Captured)
		{
			HandleCameraMovement((InputEventMouseMotion)@event);
		}
		else if (Input.IsActionJustPressed("ui_cancel"))
		{
			var mouse_mode = Input.GetMouseMode();
			if (mouse_mode == Input.MouseModeEnum.Visible)
			{
				Input.SetMouseMode(Input.MouseModeEnum.Captured);
			}
			else if (mouse_mode == Input.MouseModeEnum.Captured)
			{
				Input.SetMouseMode(Input.MouseModeEnum.Visible);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Direction = ProcessDirection();
		ProcessMovement(delta);
		GD.Print(delta, " | PLAYER");
	}

    public override void _Process(double delta)
    {
        if (HeadTilt)
		{
			float x_input = Input.GetAxis("ui_leftside", "ui_rightside");
			Head.Call("cam_tilt", x_input, delta);
		}

		if (weaponTilt)
		{
			Weapon.Call("weapon_tilt", CameraVelocity.X, CameraVelocity.Y, delta);
		}
    }



    //	---
    //		PRIVATE FUNCTIONS
    // 	---

    /// <summary>
    /// Handles the movement of the head
    /// and the camera. Also clamps the movement of the
    /// camera
    /// </summary>
    /// <param name="event">The <c>InputEventMouseMotion event</c>, derivative from the <c>InputEvent</c> type</param>
    private void HandleCameraMovement(InputEventMouseMotion @event)
	{
		RotateY(Mathf.DegToRad(-@event.Relative.X * CameraSensitivity));
		Head.RotateX(Mathf.DegToRad(-@event.Relative.Y * CameraSensitivity));

		if (weaponTilt)
		{
			CameraVelocity.X = -@event.Relative.X * CameraSensitivity;
			CameraVelocity.Y = -@event.Relative.Y * CameraSensitivity;
		}

		var newRotation = Head.Rotation;
		newRotation.X = Mathf.Clamp(Head.Rotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
		Head.Rotation = newRotation;
	}

	private Vector3 ProcessDirection()
	{
		Vector3 ReturnDirection = Vector3.Zero;

		if (Input.IsActionPressed("ui_forward"))
		{
			ReturnDirection -= Transform.Basis.Z;
		}
		else if (Input.IsActionPressed("ui_backward"))
		{
			ReturnDirection += Transform.Basis.Z;
		}

		if (Input.IsActionPressed("ui_leftside"))
		{
			ReturnDirection -= Transform.Basis.X;
		}
		else if (Input.IsActionPressed("ui_rightside"))
		{
			ReturnDirection += Transform.Basis.X;
		}

		return ReturnDirection;
	}

	private void ProcessMovement(double delta)
	{
		var WishDir = Direction.Normalized();

		if (IsOnFloor())
		{
			Velocity = UpdateVelocityGround(WishDir, delta);
		}
		else
		{
			Velocity = UpdateVelocityAir(WishDir, delta);
		}

		HandleJumpInput(delta);

		Velocity = UpdateVelocityGravityJump(delta);
		MoveAndSlide();
	}

	private void HandleJumpInput(double delta)
	{
		if (Input.IsActionJustPressed("ui_jump"))
		{
			JumpBufferReset();
		}
		if (Input.IsActionPressed("ui_jump"))
		{
			JumpBufferStep(delta);
		}
		if (Input.IsActionJustReleased("ui_jump"))
		{
			ConsumeJump();
		}
	}

	private Vector3 UpdateVelocityGravityJump(double delta)
	{
		var vel = Velocity;

		if (IsOnFloor())
		{
			CoyoteBufferReset();
		}
		else
		{
			CoyoteBufferStep(delta);
		}

		if (CanJump())
		{
			ConsumeJump();
			vel.Y = JumpForce;
		}

		vel.Y -= Gravity * (float)delta;

		return vel;
	}


	private Vector3 UpdateVelocityGround(Vector3 wishDir, double delta)
	{
		var speed = Velocity.Length();

		if (speed != 0)
		{
			var control = MathF.Max(1.5f, speed);
			var drop = control * FRICTION * (float)delta;

			Velocity *= Mathf.Max(speed - drop, 0) / speed;
		}

		return Accelerate(wishDir, MAX_GROUND_SPEED, delta);
	}

	private Vector3 UpdateVelocityAir(Vector3 wishDir, double delta)
	{
		return Accelerate(wishDir, MAX_AIR_SPEED, delta);
	}

	public void ApplyVelocity(Vector3 wishDir, double delta)
	{
		Vector3 WishDir;
		if (wishDir.IsNormalized())
		{
			WishDir = wishDir;
		}
		else
		{
			WishDir = wishDir.Normalized();
		}

		
		if (IsOnFloor())
		{
			Velocity = UpdateVelocityGround(WishDir, delta);
		}
		else
		{
			Velocity = UpdateVelocityAir(WishDir, delta);
		}
	}

	private Vector3 Accelerate(Vector3 wishdir, float max_velocity, double delta)
	{
		var current_speed = Velocity.Dot(wishdir);

		var add_speed = Mathf.Clamp(max_velocity - current_speed, 0, MAX_ACCELERATION * (float)delta);

		return Velocity + add_speed * wishdir;
	}

	private bool CanJump()
	{
		if (JumpBufferCurrent <= 0 || CoyoteBufferCurrent <= 0)
		{
			return false;
		}
		return true;
	}

	private void ConsumeJump()
	{
		JumpBufferCurrent = 0;
		CoyoteBufferCurrent = 0;
	}

	private void JumpBufferStep(double delta)
	{
		JumpBufferCurrent = Math.Max(JumpBufferCurrent - delta, 0);
	}

	private void JumpBufferReset()
	{
		JumpBufferCurrent = JumpBufferUpLimit;
	}

	private void CoyoteBufferStep(double delta)
	{
		CoyoteBufferCurrent = Math.Max(CoyoteBufferCurrent - delta, 0);
	}

	private void CoyoteBufferReset()
	{
		CoyoteBufferCurrent = CoyoteBufferUpLimit;
	}
}
