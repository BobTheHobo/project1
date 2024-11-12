using Godot;
using System;

public partial class player : CharacterBody2D
{
    public int health = 100;
    public bool player_alive = true;


    public const float Speed = 200.0f;
    public const float JumpVelocity = -300.0f;

    public string Attack_Type;
    public bool current_attack = false;

    public AnimatedSprite2D animatedSprite;
    public CollisionShape2D attack_zone;

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
            animatedSprite.FlipH = true;

            Vector2 position = attack_zone.Position;
            position.X = -Math.Abs(position.X);
            attack_zone.Set("position", position);
            GD.Print("Left Position: ", position.X);
        }
        else
        {
        
            animatedSprite.FlipH = false;

            Vector2 position = attack_zone.Position;
            position.X = Math.Abs(position.X);
            attack_zone.Set("position", position);
            GD.Print("Right Position: ", position.X);
        }

        Velocity = velocity;
        MoveAndSlide();

        if (velocity.X != 0 && !current_attack)
        {
            velocity.X = direction.X * Speed;
            animatedSprite.Play("Run");
        }
        if (velocity.X == 0 && !current_attack)
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
            animatedSprite.Play("Idle");
        }
        
        //Attack input and attack type assignment
        
        if (Input.IsActionPressed("Basic Attack"))
        { 
            current_attack = true;
            Attack_Type = "Basic Attack";
            _Attack_Animation(Attack_Type);
        }

        if (Input.IsActionPressed("Heavy Attack"))
        {
            current_attack = true;
            Attack_Type = "Heavy Attack";
            _Attack_Animation(Attack_Type);
        }

        if (Input.IsActionPressed("Special Attack"))
        {
            current_attack = true;
            Attack_Type = "Special Attack";
            _Attack_Animation(Attack_Type);
        }
        
    }

    public void _Attack_Animation(string Attack_Type)
    {
        if (current_attack)
        {
            if (Attack_Type == "Basic Attack" || Attack_Type =="Heavy Attack" || Attack_Type == "Special Attack")
            {
                animatedSprite.Play(Attack_Type);
            }
        }
    }

    public void _on_animated_sprite_2d_animation_finished()
    {
        current_attack = false;
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
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        attack_zone = GetNode<Area2D>("Attack_Zone").GetChild<CollisionShape2D>(0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
