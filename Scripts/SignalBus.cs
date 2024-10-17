using Godot;
using System;

// define global signals needed here
public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }

    // Creates a singleton instance of SignalBus
    // Allows access to singleton without making an instance or typecasting
    public override void _Ready()
    {
        Instance = this;
    }

    [Signal]
    public delegate void EnemyEnteredAttackRangeEventHandler(Node2D enemy);

    [Signal]
    public delegate void EnemyLeftAttackRangeEventHandler(Node2D enemy);

    // Combat UI signals
    [Signal]
    public delegate void ShowEnemyInRangeUIEventHandler(bool show);
}
