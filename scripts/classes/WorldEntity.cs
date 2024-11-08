using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;

[GlobalClass]
public partial class WorldEntity : CharacterBody3D
{
    
    private List<IComponent> _components = new();

    public EventBus EventBus { get; } = new();

    [Export]
    public bool Debug = false;

    public override void _Ready()
    {
        _components = GetChildren().OfType<IComponent>().ToList();
        EventBus.DebugEventBus = Debug;

        foreach (var item in _components)
        {
            // Print all the items in _components in the Godot console
            GD.Print(item.GetType().Name);
            item.Entity = this;
            item.Init();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
        // Call the Update method on all the components that have a TypeOfUpdate set as Physics
        foreach (var component in _components.Where(c => c.TypeOfUpdate == TYPE_OF_UPDATE.PHYSICS))
        {
            component.Update(delta);
        }
    }

    public override void _Process(double delta)
    {
        foreach (var component in _components.Where(c => c.TypeOfUpdate == TYPE_OF_UPDATE.PROCESS))
        {
            component.Update(delta);
        }
    }
}
