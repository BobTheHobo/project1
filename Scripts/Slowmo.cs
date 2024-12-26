using Godot;
using System;


public partial class Slowmo : Node
{
	private static readonly float DefaultSlowSpeed = 0.25f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Makes the game run at the default slow speed
	public static void SlowmoOn()
	{
		Engine.TimeScale = DefaultSlowSpeed;
	}

	// Overridden method to specify speed
	public static void SlowmoOn(float speed)
	{
		Engine.TimeScale = speed;
	}

	// Slowmo off
	public static void SlowmoOff()
	{
		Engine.TimeScale = 1;
	}
}
