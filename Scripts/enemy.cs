using Godot;
using System;

public partial class enemy : CharacterBody2D
{
    private int _speed = 35;
    private bool _playerChase = false;
    private Node2D _player = null; // target player
    private bool _playerInAttackRange = false; //if player is in attack range
    private int _direction = 1;
    private Random _rng = new Random();
    private int[] _attackSequence;
    private AnimatedSprite2D _sprite;

    // Method to signifiy that this is an enemy, DON'T DELETE
    public void IsEnemy() { }

    private string GetAttackSequenceString()
    {
        string sequence = "";
        for (int i = 0; i < _attackSequence.Length; i++)
        {
            sequence += _attackSequence[i];
        }
        return sequence;

    }

    private void GenerateAttackSequence(int numAttacks)
    {
        _attackSequence = new int[numAttacks];
        for (int i = 0; i < numAttacks; i++)
        {
            _attackSequence[i] = _rng.Next(0, 3); // Generate random number between 0 and 2
        }
        GD.Print("Attack sequence generated:", GetAttackSequenceString());
    }

    private void HandleAnimations()
    {
        if (_playerInAttackRange)
        {
            _sprite.Play("attack");
        }
        else
        {
            _sprite.Play("idle");
        }
    }

    private void MoveEnemy()
    {
        if (!IsOnFloor())
        {
            Velocity += GetGravity();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleAnimations();
        MoveEnemy();

        if (_playerChase)
        {
            float newPos = Position.X + (_player.Position.X - Position.X) / _speed;
            Position = Position with { X = newPos };

            if (_player.Position.X - Position.X < 0)
            {
                _sprite.FlipH = true;
            }
            else
            {
                _sprite.FlipH = false;
            }
        }

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
        }
    }

    private void _on_attack_range_body_exited(Node2D body)
    {
        if (body == _player)
        {
            _playerInAttackRange = false;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        GenerateAttackSequence(6);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
