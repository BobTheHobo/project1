using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Hitbox : Area2D
{
	[Export]
	public int damage = 1;

	public List<Hurtbox> hurtboxes = new();

	private bool isPlayerHitbox;

    public override void _Ready()
    {
        base._Ready();
		CollisionLayer = 0;
		CollisionMask = 3; // Only touch objects in layer 3
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
    }

	// Virtual to allow for override in child classes
    public virtual void OnAreaEntered(Node2D node)
    {
        if (node is not Hurtbox)
        {
            return;
        }

		// Ignore hurtboxes if they belong to same owner
		if (node.Owner == Owner)
		{
			return;
		}


		hurtboxes.Add((Hurtbox)node);

        // if (Owner.HasMethod("HandleDamage"))
        // {
        //     Owner.Call("HandleDamage", damage);
        // }
    }

    public virtual void OnAreaExited(Node2D node)
    {
        if (node is not Hurtbox)
        {
            return;
        }

		// Ignore hurtboxes if they belong to same owner
		if (node.Owner == Owner)
		{
			return;
		}

		hurtboxes.Remove((Hurtbox)node);
    }
}
