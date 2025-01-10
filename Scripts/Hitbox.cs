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
		// Hitbox and hurtbox collisions are reversed in order for hurtbox to call take_damage function on parent
		CollisionLayer = 0;
		CollisionMask = 3; // Only touched objects in layer 3
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;

		if (Owner is player)
		{
			isPlayerHitbox = true;
		}
		else
		{
			isPlayerHitbox = false;
		}
    }

	// Virtual to allow for override in child classes
    public virtual void OnAreaEntered(Node2D node)
    {
        if (node is not Hurtbox)
        {
            return;
        }

		// Ignore player's hitboxes
		if (node.Owner is player && isPlayerHitbox)
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

		// Ignore player's hitboxes
		if (node.Owner is player && isPlayerHitbox)
		{
			return;
		}

		hurtboxes.Remove((Hurtbox)node);
    }
}
