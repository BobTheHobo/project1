using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

public partial class enemy : CharacterBody2D
{
    [Export]
    public int health = 10;
    [Export]
    public int damage = 1;
    private slowableNode _slow;
    public slowableNode SlowNode
    {
        get
        { return _slow; }
        private set {}
    }

    public const float DefaultSpeed = 100.0f;
    public float Speed = DefaultSpeed;
    public int direction_facing = 1; //1 is right, -1 is left

    private Vector2 _gravity;
    private bool _playerChase = false;
    private Node2D _player = null; // target player
    private bool _playerInAttackRange = false; //if player is in attackable range
    private bool _playerInAttackHitbox = false; //if player is in actual hitbox
    private int _direction = 1;
    private Random _rng = new Random();
    private Stack<Combat.AttackType> _attackSequence;
    private Combat.AttackType _currentAttack;
    private bool _isAttacking = false;
    private float cooldown = 1; // Cooldown between attacks
    private AnimatedSprite2D _sprite;
    private AnimatedSprite2D _attackSprite;
    private Hitbox _attackHitbox;
    private CollisionShape2D _attackHitboxColShape;
    private CollisionShape2D _attackRange;

    // Related to enemy UI
    private EnemyUiControl _uiControl;
    private Label _attackLabel;
    private Label _healthLabel;
    private EnemyAttackTimer _attackTimer;

    // Method to signifiy that this is an enemy, DON'T DELETE
    public void IsEnemy() { }

    public void DisplayEnemyUI(bool show)
    {
        DisplayAttackTimer(show);
        if (show)
        {
            DisplayCurrentAttack();
            _healthLabel.Visible = true;
        }
        else
        {
            HideCurrentAttack();
            _healthLabel.Visible = false;
        }
    }

    private void DisplayAttackTimer(bool show)
    {
        if (show)
        {
            _attackTimer.Visible = true;            
        }
        else
        {
            _attackTimer.Visible = false;            
        }
    }

    private void DisplayCurrentAttack()
    {
        //GD.Print("Current enemy attack: " + Combat.GetAttackString(_currentAttack));

        // Overwrites whatever text is currently there
        //string newText = _currentAttack.ToString();
        string newText = "Attack: " + Combat.GetCurrentAttack(_attackSequence).ToString();
        _attackLabel.SetText(newText);

        _attackLabel.SetVisible(true);
    }

    private void UpdateAttackDisplay()
    {
        string newText = "Attack: " + _currentAttack.ToString();
        _attackLabel.SetText(newText);
    }

    private void UpdateHealthDisplay()
    {
        string newText = "Health: " + health.ToString();
        _healthLabel.SetText(newText);
    }
    
    private void HideCurrentAttack()
    {
        _attackLabel.SetVisible(false);
    }


    private void HandleAnimations()
    {
        if (_playerInAttackRange)
        {
            _sprite.Play("attack");
            HandleAttacks();
        }
        else
        {
            _sprite.Play("idle");
        }
    }

    public void HandleDamage(int amount)
    {
        health -= amount;
        UpdateHealthDisplay();

        if (health <= 0)
        {
            QueueFree();    
        }
    }

    // Plays animations and handles attacks
    private void HandleAttacks()
    {
        if (!_isAttacking && _currentAttack != Combat.AttackType.None)
        {
            _attackTimer.RunTimer(cooldown);
            _isAttacking = true;
            switch (_currentAttack)
            {
                case Combat.AttackType.Light:
                    _attackSprite.Play("Basic Attack");
                    break;
                case Combat.AttackType.Heavy:
                    _attackSprite.Play("Heavy Attack");
                    break;
                case Combat.AttackType.Special:
                    _attackSprite.Play("Special Attack");
                    break;
            }
        }
        UpdateAttackDisplay();
    }

    // Signal called when attack sprite animation is done
    public void _on_attack_sprite_2d_animation_finished()
    {
        // use custom timer for cooldown (to account for slowdown as well)
        // _attackTimer.RunTimer(cooldown);
        // GD.Print("Start Timer");
    }

    private void OnAttackTimerTimeout()
    {
        foreach (Hurtbox hurtbox in _attackHitbox.hurtboxes)
        {
            GD.Print("Enemy is hurting " + hurtbox.Name);

            // Handle damage
            if (hurtbox.Owner.HasMethod("HandleDamage"))
            {
                hurtbox.Owner.Call("HandleDamage", damage);
            }
        }

        _isAttacking = false;
        // Set next attack
        _currentAttack = Combat.GetNextAttack(_attackSequence);
        //GD.Print("Next attack: " + _currentAttack.ToString());

        if (_currentAttack == Combat.AttackType.None)
        {
            _attackSprite.Visible = false;
        }
    }

    private void MoveEnemy(double delta)
    {
        Vector2 velocity = Velocity;

        // Vertical movement
        if (!IsOnFloor())
        {
            velocity.Y += _gravity.Y * (float)delta;
        }

        // Chase Player
        if (_playerChase)
        {
            // direction: 1 is right, -1 is left, 0 is stationary
            direction_facing = Mathf.Sign(_player.Position.X - Position.X);
            if (direction_facing != 0)
            {
                velocity.X = direction_facing * Speed;
            }
        }
        else // Slow down horizontally
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        // Flip model
        if (velocity.X < 0) // Facing right
        {
            direction_facing = -1;

            FlipShape(_attackHitboxColShape, -1);
            FlipShape(_attackRange, -1);
            FlipSprite(_attackSprite, -1);
            FlipSprite(_sprite, -1);
        }
        else if (velocity.X > 0) // Facing left
        {
            direction_facing = 1;

            FlipShape(_attackHitboxColShape, 1);
            FlipShape(_attackRange, 1);
            FlipSprite(_attackSprite, 1);
            FlipSprite(_sprite, 1);
        }

        Velocity = velocity;
    }

    private void FlipSprite(AnimatedSprite2D sprite)
    {
        FlipSprite(sprite, direction_facing);
    }

    // Only works with animatedsprite2d right now but we can always fix later
    private void FlipSprite(AnimatedSprite2D sprite, int direction)
    {
        if (direction >= 0)
        {
            sprite.FlipH = false;

            Vector2 spriteOffset = sprite.Offset;
            if (spriteOffset.X != 0)
            {
                spriteOffset.X = Math.Abs(spriteOffset.X);
                sprite.Set("offset", spriteOffset);
            }
        }
        else
        {
            sprite.FlipH = true;
            Vector2 spriteOffset = sprite.Offset;

            if (spriteOffset.X != 0)
            {
                spriteOffset.X = -Math.Abs(spriteOffset.X);
                sprite.Set("offset", spriteOffset);
            }
        }
    }

    private void FlipShape(CollisionShape2D thing)
    {
        FlipShape(thing, direction_facing);
    }

    private void FlipShape(CollisionShape2D thing, int direction)
    {
        if (direction >= 0)
        {
            Vector2 position = thing.Position;
            position.X = Math.Abs(position.X);
            thing.Set("position", position);
        }
        else
        {
            Vector2 position = thing.Position;
            position.X = -1 * Math.Abs(position.X);
            thing.Set("position", position);
        }
    }

    public void SlowEnemy(float slowFactor)
    {
        Speed = DefaultSpeed * slowFactor; 
        _gravity = GetGravity() * slowFactor;
        _sprite.SpeedScale = 1 * slowFactor;
        _attackSprite.SpeedScale = 1 * slowFactor;
    }

    private void HandleSlowmoChange(object sender, SlowmoController.SlowChangedEventArgs e)
    {
        // Change velocity according to slowmo
        Vector2 newVelocity = _slow.CalcVelocityOnSlowChange(e, Velocity);
        Velocity = newVelocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Slowmo handling
        delta = _slow.DeltaHandler(delta);
        SlowEnemy(_slow.LocalSlowFactor);

        MoveEnemy(delta);

        HandleAnimations();

        MoveAndSlide();
    }

    private void _on_detection_area_body_entered(Node2D body)
    {
        _player = body;
        _playerChase = true;
    }

    private void _on_detection_area_body_exited(Node2D body)
    {
        _player = null;
        _playerChase = false;
    }

    private void _on_attack_range_body_entered(Node2D body)
    {
        if (body == _player)
        {
            _playerInAttackRange = true;
            Combat.Instance.CombatEntered(this);
            DisplayEnemyUI(true);
        }
    }

    private void _on_attack_range_body_exited(Node2D body)
    {
        Combat.Instance.CombatExited(this);
        DisplayEnemyUI(false);

        if (body == _player)
        {
            _playerInAttackRange = false;

        }
    }

    public void OnAttackHitboxEntered(Node2D body)
    {
        if (body.HasMethod("IsPlayer"))
        {
            _playerInAttackHitbox = true;
        }
    }

    public void OnAttackHitboxExited(Node2D body)
    {
        if (body.HasMethod("IsPlayer"))
        {
            _playerInAttackHitbox = false;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _slow = new slowableNode(this);

        Main.AddEnemy(this); // Make sure to add every enemy to the global list so that it can be easily tracked by combat and other scripts

        // Get sprites
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _attackSprite = GetNode<AnimatedSprite2D>("AttackSprite2D");
        _attackRange = GetNode<Area2D>("AttackRange").GetChild<CollisionShape2D>(0);
        _attackHitbox = (Hitbox) GetNode<Area2D>("AttackHitbox");
        _attackHitboxColShape = (CollisionShape2D) _attackHitbox.GetChild(0);

        // Subscribe to hitbox/hurtbox interaction
        _attackHitbox.BodyEntered += OnAttackHitboxEntered;
        _attackHitbox.BodyExited += OnAttackHitboxExited;

        // get UI control and set necessary child nodes
        _uiControl = GetNode<EnemyUiControl>("UIControl");
        _attackLabel = _uiControl.AttackTypeLabel;
        _attackTimer = _uiControl.AttackTimer;
        _healthLabel = _uiControl.HealthLabel;

        UpdateHealthDisplay();

        // Subscribe to timer timeout
        _attackTimer.AttackTimerTimeout += OnAttackTimerTimeout;

        DisplayEnemyUI(false);

        // Generate initial attack seq
        _attackSequence = Combat.Instance.GenerateAttackSequence(6);
        _currentAttack = Combat.GetCurrentAttack(_attackSequence);
        
        // Subscribe to slowmo event
        SlowmoController.GlobalSlowChanged += HandleSlowmoChange;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _ExitTree()
    {
        // Need to remove listeners b/c custom signal
        SlowmoController.GlobalSlowChanged -= HandleSlowmoChange; 
        _attackTimer.AttackTimerTimeout -= OnAttackTimerTimeout;

        Main.RemoveEnemy(this);
        base._ExitTree();
    }
}
