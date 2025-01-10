using Godot;
using System;

public partial class Hurtbox : Area2D
{
    public override void _Ready()
    {
        base._Ready();
		CollisionLayer = 3;
		CollisionMask = 0; // Don't touch anything else
    }

    
}