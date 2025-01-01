using Godot;
using System;

public partial class player : CharacterBody2D
{
    private slowableNode _slow; 
    public int health = 100;
    public bool player_alive = true;

    public Vector2 Gravity;
    public const float DefaultSpeed = 200.0f;
    public float Speed = DefaultSpeed;
    public const float DefaultJumpVelocity = -300.0f;
    public float JumpVelocity = DefaultJumpVelocity;

    public string Attack_Type;
    public bool current_attack = false;

    public AnimatedSprite2D animatedSprite;
    private AnimatedSprite2D attackSprite;
    public CollisionShape2D attackZone;

    public int direction_facing = 1; //1 is right, -1 is left

    // Method specifies that this is a player DO NOT REMOVE
    public void isPlayer()
    {
    }

    // Manipulate all necessary variables affected by the slow speed
    public void SlowPlayer(float slowFactor)
    {
        Speed = DefaultSpeed * slowFactor; 
        JumpVelocity = DefaultJumpVelocity * slowFactor;
        Gravity = GetGravity() * slowFactor;
        animatedSprite.SpeedScale = 1 * slowFactor;
        attackSprite.SpeedScale = 1 * slowFactor;
    }

    // Handle slowmo changes
    private void HandleSlowmoChange(object sender, SlowmoController.GlobalSlowChangedEventArgs e)
    {
        // calculate velocity according to slowmo
        Vector2 newVelocity = _slow.CalcVelocityOnSlowChange(e, Velocity);
        Velocity = newVelocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Lets slowmo handle delta to allow for slowing/speeding up, etc
        // Also need to affect speed
        delta = _slow.DeltaHandler(delta);
        SlowPlayer(_slow.LocalSlowFactor);

        Vector2 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            // How does this work, isn't this double dipping a change 
            //  b/c gravity is reduced AND delta is reduced during slowmo???
            velocity.Y += Gravity.Y * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
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
            // Flip attack sprite
            attackSprite.FlipH = true;
            Vector2 attackAnimPos = attackSprite.Offset;
            attackAnimPos.X = -Math.Abs(attackAnimPos.X);
            attackSprite.Set("offset", attackAnimPos);

            // Flip character sprite
            animatedSprite.FlipH = true;

            // Flip attack zone
            Vector2 position = attackZone.Position;
            position.X = -Math.Abs(position.X);
            attackZone.Set("position", position);
        }
        else
        {
            // Flip attack sprite
            attackSprite.FlipH = false;
            Vector2 attackAnimPos = attackSprite.Offset;
            attackAnimPos.X = Math.Abs(attackAnimPos.X);
            attackSprite.Set("offset", attackAnimPos);

            // Flip character sprite
            animatedSprite.FlipH = false;

            // Flip attack zone
            Vector2 position = attackZone.Position;
            position.X = Math.Abs(position.X);
            attackZone.Set("position", position);
        }

        // Set player velocity
        Velocity = velocity;
        MoveAndSlide();

        // Handle idle and non-attacking animations
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
    }

    public void _Attack_Animation(string Attack_Type)
    {
        if (current_attack)
        {
            if (Attack_Type == "Basic Attack" || Attack_Type =="Heavy Attack" || Attack_Type == "Special Attack")
            {
                attackSprite.Visible = true;
                //((CanvasItem)attackSprite).SetVisible(true);
                
                // GD.Print("Animation start");
                attackSprite.SetVisible(true);
                attackSprite.Play(Attack_Type);
            }
        }
    }

    // Signal called when attack sprite animation is done
    public void _on_attack_sprite_2d_animation_finished()
    {
        current_attack = false;

        // Hide attack animation after finished
        attackSprite.SetVisible(false);
        // GD.Print("Animation done");
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
        Main._player = this; // Sets global reference to this player instance

        _slow = new slowableNode(this);

        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        attackSprite = GetNode<AnimatedSprite2D>("AttackSprite2D");
        attackZone = GetNode<Area2D>("AttackZone").GetChild<CollisionShape2D>(0);

        // Subscribe to slowmo event
        SlowmoController.GlobalSlowChanged += HandleSlowmoChange;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        // Use combat input handler defined in Combat.cs
        Combat.Instance.CombatInputHandler(@event);

        //Attack input and attack type assignment
        if (@event.IsActionPressed("Basic Attack"))
        { 
            current_attack = true;
            Attack_Type = "Basic Attack";
            _Attack_Animation(Attack_Type);
        }

        if (@event.IsActionPressed("Heavy Attack"))
        {
            current_attack = true;
            Attack_Type = "Heavy Attack";
            _Attack_Animation(Attack_Type);
        }

        if (@event.IsActionPressed("Special Attack"))
        {
            current_attack = true;
            Attack_Type = "Special Attack";
            _Attack_Animation(Attack_Type);
        }
    }
}
