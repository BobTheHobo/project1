using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// Static class that controlls all slowmoNodes
public partial class SlowmoController : Node
{
	public static readonly float NormalExecFactor = 1; // Slow factor of normal game execution

	public static readonly float DefaultSlowFactor = 0.25f;
	public static float GlobalSlowFactor { get; private set; } = DefaultSlowFactor;
	public static bool GlobalSlowIsOn { get; private set; } = false;

	public class GlobalSlowChangedEventArgs : EventArgs
	{
		public float PreviousSlowFactor { get; set; }
		public float CurrentSlowFactor { get; set; }
		public bool PreviouslyOn { get; set; }
		public bool CurrentlyOn { get; set; } 
		public Vector2 ObjVelocity { get; set; }
	}
	
	public static event EventHandler<GlobalSlowChangedEventArgs> GlobalSlowChanged;

	protected static void OnGlobalSlowChanged(GlobalSlowChangedEventArgs e)
	{
		// Invokes event (basically sends signal that this happened)
        GlobalSlowChanged?.Invoke(null, e);
    }

	// Makes the game run at the default slow speed
	public static void GlobalSlowmoOn()
	{
		GlobalSlowmoOn(DefaultSlowFactor);
	}

	// Overridden method to specify slow factor
	public static void GlobalSlowmoOn(float factor)
	{
		Debug.Assert(factor != NormalExecFactor, "Slow factor cannot be normal exec factor, use the slowmo off method");

		// Send out event that slow was toggled
        GlobalSlowChangedEventArgs args = new()
        {
            PreviousSlowFactor = GlobalSlowFactor,
            CurrentSlowFactor = factor,
            PreviouslyOn = false,
            CurrentlyOn = true
        };
		OnGlobalSlowChanged(args);

		// Set speed
		GlobalSlowFactor = factor;
		GlobalSlowIsOn = true;
		// Engine.TimeScale = speed;
	}

	// Slowmo off (reset game speed to 1)
	public static void GlobalSlowmoOff()
	{
		// Send out event that slow was toggled
        GlobalSlowChangedEventArgs args = new()
        {
            PreviousSlowFactor = GlobalSlowFactor,
            CurrentSlowFactor = NormalExecFactor,
            PreviouslyOn = true,
            CurrentlyOn = false
        };
		OnGlobalSlowChanged(args);

		// Engine.TimeScale = 1;
		GlobalSlowFactor = NormalExecFactor;
		GlobalSlowIsOn = false;
	}

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
