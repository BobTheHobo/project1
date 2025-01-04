using Godot;
using System;

public partial class EnemyAttackTimer : CustomTimer
{
	private slowableNode _slow;
	private CustomTimer _timer;
	public double cooldown = 1f;
	public double timeRemaining;
	public float currentProgress;

	[Signal]
	public delegate void AttackTimerTimeoutEventHandler();

	public EnemyAttackTimer(slowableNode slowable, double defaultCD) : base(slowable, defaultCD)
	{
		_slow = slowable;
		cooldown = defaultCD;
	}

	// Runs a timer with default cooldown
	public void RunTimer()
	{
		RunTimer(cooldown);
	}	

	public void RunTimer(double time)
	{
		_timer = new(_slow, time);
		AddChild(_timer);
		_timer.CustomTimerTimeout += OnTimerTimeout;
	}

	public void Pause()
	{
		if (_timer != null)
		{
			_timer.Stop();
		}
		else
		{
			GD.PrintErr("No enemyAttackTimer exists");
		}
	}
	public void Resume()
	{
		if (_timer != null)
		{
			if (_timer.IsStopped())
			{
				_timer.Start();
			}
		}
		else
		{
			GD.PrintErr("No enemyAttackTimer exists");
		}
	}

	private void OnTimerTimeout()
	{
		GD.Print("Attack timer finished");
		EmitSignal(SignalName.AttackTimerTimeout);
		_timer = null; // CustomTimer will free itself after timeout
	}

	public void SetCooldown(float cd)
	{
		cooldown = cd;
	}

	public void GetProgressPercent()
	{
			
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
