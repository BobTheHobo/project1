using Godot;
using System;

public partial class player : CharacterBody2D
{
    public int health = 100;
    public bool player_alive = true;


    public const float Speed = 200.0f;
    public const float JumpVelocity = -300.0f;

    public int direction_facing = 1; //1 is right, -1 is left

    // Method specifies that this is a player DO NOT REMOVE
    public void isPlayer()
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity.Y += GetGravity() * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        float direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (direction != 0)
        {
            direction_facing = (int)Mathf.Sign(direction);
            velocity.X = direction.X * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        if (direction_facing == -1)
        {
            AnimatedSprite2D.flip_h = true;
        }
        else
        {
            AnimatedSprite2D.flip_h = false;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void OnAttackRangeBodyEntered(Node2D body)
    {
        if (body.HasMethod("enemy"))
        {
            SignalBus.enemyEnteredAttackRange.Emit(body);
        }
    }

    public void OnAttackRangeBodyExited(Node2D body)
    {
        if (body.HasMethod("enemy"))
        {
            SignalBus.enemyLeftAttackRange.Emit(body);
        }
    }
}
