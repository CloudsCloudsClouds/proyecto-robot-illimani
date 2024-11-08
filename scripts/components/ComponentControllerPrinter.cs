using Godot;
using GlobalEnums;
using System;

[GlobalClass]
public partial class ComponentControllerPrinter : IComponent
{
    public override WorldEntity Entity { get; set; }

    [Export]
    public override TYPE_OF_UPDATE TypeOfUpdate { get; set; } = TYPE_OF_UPDATE.PROCESS;


    public override void Init()
    {
        SetPhysicsProcess(false);
        SetProcessMode(ProcessModeEnum.Disabled);
        Entity.EventBus.Subscribe<EventDirection>(PrintControllerInputs);
    }

    public override void Update(double delta)
    {
        return;
    }


    void PrintControllerInputs(EventDirection @event)
    {
        GD.Print(@event.GetDirForce());
    }
}