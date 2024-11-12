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
        Godot.Vector2 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity.Y += GetGravity().Y * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Godot.Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (direction.X != 0)
        {
            direction_facing = (int)Mathf.Sign(direction.X);
            velocity.X = direction.X * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        //flip the player model if going left
        if (direction_facing == -1)
        {
            AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            animatedSprite2D.FlipH = true;
        }
        else
        {
            AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            animatedSprite2D.FlipH = false;
        }

        Velocity = velocity;
        MoveAndSlide();

        if (velocity.X != 0)
        {
            AnimatedSprite2D animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            velocity.X = direction.X * Speed;
            animatedSprite.Play("Run");
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
            AnimatedSprite2D animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            animatedSprite.Play("Idle");
        }
    }

    public void _on_attack_range_body_entered(Node2D body)
    {
        if (body.HasMethod("IsEnemy"))
        {
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.EnemyEnteredAttackRange, body);
        }
    }

    public void _on_attack_range_body_exited(Node2D body)
    {
        if (body.HasMethod("IsEnemy"))
        {
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.EnemyLeftAttackRange, body);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
