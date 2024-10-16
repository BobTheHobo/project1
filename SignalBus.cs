using Godot;
using System;

public partial class SignalBus : Node
{
    [Signal]
    public delegate void EnemyEnteredAttackRangeEventHandler (Node2D enemy);

    [Signal]
    public delegate void EnemyLeftAttackRangeEventHandler (Node2D enemy);
        
    [Signal]
    public delegate void ShowEnemyInRangeUIEventHandler (bool show);
}
