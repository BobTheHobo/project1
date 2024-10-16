using Godot;
using System;

public partial class CombatUi : CanvasLayer
{
	private Label enemyInRangeUI;
	
	// Called when the node enters the scene tree for the first time.
	public void _assignElement()
	{
		enemyInRangeUI = GetNode<Label>("EnemyInRangeLabel");
	}

	public void connectToSignals()
	{
		//SignalBus signalBus = GetNode = GetNode<SignalBus>(SignalBus);
		SignalBus.ShowEnemyInRangeUI += _show_enemy_in_range_ui;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void _ready()
	{
		_assignElement();
		connectToSignals();

		enemyInRangeUI.Visible = false;
	}

	public void _show_enemy_in_range_ui(bool visible)
	{
		if (visible)
		{
			enemyInRangeUI.Visible = true;
		}
		else
		{
			enemyInRangeUI.Visible = false;
		}
	}
}
