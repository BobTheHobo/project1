using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

using _controller = SlowmoController;

/*
Node that is meant to be instantiated in whatever class is to be slowable

I could have other classes inherit this, but then I would have to make separate middleman slowmo classes that each individual class inherits (e.g., CharacterBody2D <- CharacterBodySlowmo <- player vs. Sprite2D <- SpriteSlowmo <- item) because c# doesn't have multiple inheritance.
Could also make it an interface but then I have to make another class that implements the interface and then use that class inside the classes that I want slowmo in which seems like more of a hassle.
Just easier to make an object that classes can define whenever they need and control
*/
public partial class slowableNode : Node
{
	public float LocalSlowFactor {get; private set;} = _controller.NormalExecFactor;
	public bool LocalSlowIsOn {get; private set;} = false;

	public slowableNode()
	{
		_controller.GlobalSlowChanged += HandleSlowChange;
		GD.Print("added");
	}

	// This will only run if the slowableNode is ADDED TO THE SCENE 
	// (i.e. added as a child to something)
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Turn local slowmo on
	public void SetLocalSlowmo(float factor)
	{
		// Set speed
		LocalSlowFactor = factor;
		
		// normalexecfactor is basically just turning off slow 
		LocalSlowIsOn = factor != _controller.NormalExecFactor;
	}

	// Triggered whenever slow is globally changed 
	// WILL OVERWRITE LOCAL SLOW !!
	private void HandleSlowChange(object sender, _controller.GlobalSlowChangedEventArgs e)
	{
		if (e.CurrentlyOn)
		{
			SetLocalSlowmo(e.CurrentSlowFactor);
		}
		else
		{
			LocalSlowmoOff();
		}
	}


	// Local slowmo off (reset this node's speed to normal exec speed)
	public void LocalSlowmoOff()
	{
		LocalSlowFactor = _controller.NormalExecFactor;
		LocalSlowIsOn = false;
	}

	// Handles delta scaling
	public double DeltaHandler(double delta)
	{
		if (LocalSlowIsOn) 
		{
			return delta * LocalSlowFactor;	
		}
		else
		{
			return delta;
		}
	}
}
