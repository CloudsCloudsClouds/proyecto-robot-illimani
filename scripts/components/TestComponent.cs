using GlobalEnums;
using Godot;

[GlobalClass]
public partial class TestComponent : IComponent
{
    public override WorldEntity Entity { get; set; }

    [Export]
    public override TYPE_OF_UPDATE TypeOfUpdate { get; set; } = TYPE_OF_UPDATE.PROCESS;

    public override void Init()
    {
        SetPhysicsProcess(false);
        SetProcessMode(ProcessModeEnum.Disabled);
    }

    public override void Update(double delta)
    {   
        GD.Print("hello! This is delta: ", delta, " And this is the entity: ", Entity.GetType());
    }
}
