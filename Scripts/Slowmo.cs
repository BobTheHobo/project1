using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Text;


public partial class Slowmo : Node
{
	private static readonly float NormalSpeed = 1;
	private static readonly float DefaultSlowSpeed = 0.25f;
	public static float CurrentSlowSpeed {get; set;} = 1;
	public static bool SlowIsOn {get; set;} = false;

	public class SlowToggledEventArgs : EventArgs
	{
		public float PreviousSpeed { get; set; }
		public float CurrentSpeed { get; set; }
		public bool PreviouslyOn { get; set; }
		public bool CurrentlyOn { get; set; } 
	}

	public static event EventHandler<SlowToggledEventArgs> SlowToggled;

	// Invokes event (basically sends signal that this happened)
	protected static void OnSlowToggled(SlowToggledEventArgs e)
	{
        SlowToggled?.Invoke(null, e);
    }

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
		SlowmoOn(DefaultSlowSpeed);
	}

	private static SlowToggledEventArgs GenerateSlowEventArgs(float previousSpeed, float currentSpeed, bool previouslyOn, bool currentlyOn)
	{
		SlowToggledEventArgs args = new SlowToggledEventArgs();
		args.PreviousSpeed = previousSpeed;
		args.CurrentSpeed = currentSpeed;
		args.PreviouslyOn = previouslyOn;
		args.CurrentlyOn = currentlyOn;
		return args;
	}

	// Overridden method to specify speed
	public static void SlowmoOn(float speed)
	{
		// Send out event that slow was toggled
		SlowToggledEventArgs args = GenerateSlowEventArgs(CurrentSlowSpeed, speed, false, true);
		OnSlowToggled(args);

		// Set speed
		CurrentSlowSpeed = speed;
		SlowIsOn = true;
		// Engine.TimeScale = speed;
	}

	// Slowmo off (reset game speed to 1)
	public static void SlowmoOff()
	{
		// Send out event that slow was toggled
		SlowToggledEventArgs args = GenerateSlowEventArgs(CurrentSlowSpeed, NormalSpeed, true, false);
		OnSlowToggled(args);

		// Engine.TimeScale = 1;
		CurrentSlowSpeed = NormalSpeed;
		SlowIsOn = false;
	}

	public static double DeltaHandler(double delta)
	{
		if (SlowIsOn) 
		{
			return delta * CurrentSlowSpeed;	
		}
		else
		{
			return delta;
		}
	}
}
