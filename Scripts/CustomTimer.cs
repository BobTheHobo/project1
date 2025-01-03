using Godot;
using System;

// Timer that also takes into account slowmo system
// Needs to be passed a slowableNode instance so that it knows
// what slow factor to use. 
// If null is passed for slowableNode then relies on globalslow value
//	in the slowmocontroller
public partial class CustomTimer : Timer
{
	private slowableNode _slowNode = null;
	public double duration = 1;
	private double adjustedDuration;
	private bool _useGlobalSlow = true;
	private double _remainingTime;
	private float previous_slow_factor;

	private Timer _timer;

	[Signal]
	public delegate void CustomTimerTimeoutEventHandler();

	// Creates a custom timer on top of GD's own timer
	public CustomTimer(slowableNode slow, double reqDuration)
	{
        _timer = new();
		AddChild(_timer);
		_timer.OneShot = true;
		_timer.Autostart = true;
		_timer.Timeout += OnTimerTimeout;

		duration = reqDuration;
		adjustedDuration = reqDuration;

		if (slow != null) // Using local slow
		{
			_useGlobalSlow = false;
			_slowNode = slow;
			_slowNode.LocalSlowChanged += HandleSlowmoChange;

			if (_slowNode.LocalSlowIsOn)
			{
				adjustedDuration *= 1.0/_slowNode.LocalSlowFactor;
			}
		}
		else // Using global slow
		{
			SlowmoController.GlobalSlowChanged += HandleSlowmoChange;

			if (SlowmoController.GlobalSlowIsOn)
			{
				adjustedDuration *= 1.0/SlowmoController.GlobalSlowFactor;
			}
		}

		_timer.WaitTime = adjustedDuration;
	}	

	private void HandleSlowmoChange(object sender, SlowmoController.SlowChangedEventArgs e)
	{
		if (e.PreviousSlowFactor <= 0) // Ignore division by 0 and negative
		{
			return;
		}

		_remainingTime = _timer.TimeLeft;
		_timer.Stop();

		//GD.Print("Stopping timer at: " + _remainingTime.ToString());

		// I'm assuming that when slow is turned off the slow factor is reset
		// to the normal exec value so i'm not checking if slow is on or off here...

		// Need to redo affects of previous slow factor and then apply 
		// current scale factor or else it'll just apply the slow twice
		_timer.WaitTime = _remainingTime * e.PreviousSlowFactor / e.CurrentSlowFactor;

		//GD.Print("New time after slow change: " + _timer.WaitTime.ToString());

		_timer.Start();
	}

	public void OnTimerTimeout()
	{
		_timer.Stop();
		GD.Print("Timer finished");
		EmitSignal(SignalName.CustomTimerTimeout);

		// Remove this timer after it's used
		QueueFree();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}    

	// Disconnect signals upon removal
	public override void _ExitTree()
    {
        if (!_useGlobalSlow && _slowNode != null)
        {
            _slowNode.LocalSlowChanged -= HandleSlowmoChange;
        }
        else
        {
            SlowmoController.GlobalSlowChanged -= HandleSlowmoChange;
        }
    }
}
