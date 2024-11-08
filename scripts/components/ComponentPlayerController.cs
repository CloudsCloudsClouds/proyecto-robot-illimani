using GlobalEnums;
using Godot;
using System;

[GlobalClass]
public partial class ComponentPlayerController : IComponent
{
    public override WorldEntity Entity { get; set; }

    [Export]
    public override TYPE_OF_UPDATE TypeOfUpdate { get; set; } = TYPE_OF_UPDATE.PROCESS;

    [Export]
    public bool Debug = false;
    [Export]
    public Camera3D Camera;


    private OBJECT_STATES _object_state = OBJECT_STATES.IDLE;

    public override void Init()
    {
        SetPhysicsProcess(false);
        Entity.EventBus.Subscribe<EventObjectState>(UpdateObjectState);
        SetProcessMode(ProcessModeEnum.Disabled);
    }

    public override void Update(double delta)
    {   
        PublishDirection();
        PublishJumpAction();
    }

    void PublishDirection()
    {
        var dirs = Input.GetVector("ui_up", "ui_down", "ui_left", "ui_right");
        var direction = new Vector3(dirs.Y, 0, dirs.X);

        if (Debug)
        {
            GD.Print("hello! This is Direction: ", direction, " And this is the entity: ", Entity.GetType());
            Entity.EventBus.Ping();
        }

        var cam_dir = Camera.GlobalTransform.Basis.Z;
        direction = direction.Rotated(Vector3.Up, Camera.Rotation.Y);
        EventDirection new_message = new EventDirection(direction, direction.Length());
        Entity.EventBus.Publish(new_message);
    }


    void PublishJumpAction()
    {
        var message = new EventInputAction(INPUT_ACTION.JUMP, GetActionState("ui_jump"));
        Entity.EventBus.Publish(message);
    }

    LIFECYCLE_STATE GetActionState(string action)
    {
        if (Input.IsActionJustPressed(action)) 
            return LIFECYCLE_STATE.JUST_ENTERED;
        if (Input.IsActionPressed(action)) 
            return LIFECYCLE_STATE.ACTIVE;
        if (Input.IsActionJustReleased(action)) 
            return LIFECYCLE_STATE.JUST_EXITED;
        return LIFECYCLE_STATE.NOT_ACTIVE; 
    }


    

    void UpdateObjectState(EventObjectState new_state)
    {
        _object_state = new_state.State;
    }
}