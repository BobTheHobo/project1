using Godot;
using System;

public partial class LockonUi : Node2D
{
	private double time = 0;
	private double rotateInterval = 0.7;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RotateSprite(delta);
	}

	// Rotates sprite 45 degrees based on a rotateInterval
	private void RotateSprite(double delta)
	{
		time += delta;
		if (time >= rotateInterval) {
			time = 0;
			this.RotationDegrees += 45;
		}
	}
}
