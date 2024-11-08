using GlobalEnums;
using Godot;
using System;

[GlobalClass]
public abstract partial class IComponent : Node3D
{
    public abstract WorldEntity Entity
    {
        set;
        get;
    }

    public abstract TYPE_OF_UPDATE TypeOfUpdate
    {
        set;
        get;
    }

    
    abstract public void Init();

    
    abstract public void Update(double delta);
}