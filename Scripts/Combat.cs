using Godot;
using System;

// Controls everything combat related, including the combat UI
// EIR = enemy in range
public partial class Combat : Node
{
    private Node2D _player; // current player
    private Node2D _targetedBody = null; // body that the player is currently targeting
    private bool _inEnemyRange = false; // whether or not player is in range of an enemy

    System.Collections.ArrayList _enemiesInRange; // Holds enemies that are in range of player


    private bool _enemyInRange = false; // if any enemy is in player attack range
    private bool _enemyInRangeIndicator = false; // indicates that an enemy is in range

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
        _enemiesInRange = new System.Collections.ArrayList();
        ConnectToSignals();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
