using Godot;
using System;

// Controls everything combat related, including the combat UI
// EIR = enemy in range
[GlobalClass]
public partial class Combat : Node
{
    // Defines singleton instance of Combat
    public static Combat Instance { get; private set; }
    private Node2D _player; // current player
    private Node2D _targetedBody = null; // body that the player is currently targeting
    private bool _inEnemyRange = false; // whether or not player is in range of an enemy

    public enum AttackType // Defines the attack type
    {
        None = 0,
        Rock = 1,
        Paper = 2,
        Scissor = 3
    }

    System.Collections.ArrayList _enemiesInRange; // Holds enemies that are in range of player


    private bool _enemyInRange = false; // if any enemy is in player attack range
    private bool _enemyInRangeIndicator = false; // indicates that an enemy is in range
    public string GetAttackString(AttackType attackType)
    {
        switch(attackType)
        {
            case AttackType.None:
            {
                return "None";
            }
            case AttackType.Rock:
            {
                return "Rock";
            }
            case AttackType.Paper:
            {
                return "Paper";
            }
            case AttackType.Scissor:
            {
                return "Scissor";
            }
            default: // Catch all, shouldn't be triggered?
            {
                return "Undefined";
            }
        } 
    }

    private void ConnectToSignals()
    {
        SignalBus.Instance.EnemyEnteredAttackRange += OnEnemyEnteredAttackRange;
        SignalBus.Instance.EnemyLeftAttackRange += OnEnemyLeftAttackRange;
    }

    private void OnEnemyEnteredAttackRange(Node2D body)
    {
        AddEnemyInRange(body);
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.ShowEnemyInRangeUI, true);
    }

    private void OnEnemyLeftAttackRange(Node2D body)
    {
        RemoveEnemyInRange(body);
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.ShowEnemyInRangeUI, false);
    }

    // Adds enemy to enemy in range array
    private void AddEnemyInRange(Node2D enemy)
    {
        _enemiesInRange.Add(enemy);
        _enemyInRange = true;
        GD.Print("Enemy entered player atrange " + enemy);
    }

    // Checks if enemy is in enemy array and removes if they are
    private void RemoveEnemyInRange(Node2D enemy)
    {
        _enemiesInRange.Remove(enemy); // No exception will be thrown if enemy can't be found so watch out
        _enemyInRange = IsAnyEnemyInRange();
    }

    // Returns if the enemies in range array is empty (no enemies in attack range)
    private bool IsAnyEnemyInRange()
    {
        return _enemiesInRange.Count != 0;
    }

    // Sets player's current target
    private void SetTarget(Node2D target)
    {
        _targetedBody = target;
    }

    // Clears player's current target
    private void ClearTarget()
    {
        _targetedBody = null;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this; // Needed to link this instance as singleton instance
        _enemiesInRange = new System.Collections.ArrayList();
        ConnectToSignals();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
