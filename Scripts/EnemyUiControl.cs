using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class EnemyUiControl : Control
{
	public EnemyAttackTimer AttackTimer { get; private set; }
	public Label AttackTypeLabel { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NodePath attackTimerPath = new("HBoxContainer/Control/AttackTimer");
		NodePath attackTypeLabelPath = new("HBoxContainer/AttackType");

		AttackTimer = GetNode<EnemyAttackTimer>(attackTimerPath);
		AttackTypeLabel = GetNode<Label>(attackTypeLabelPath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
