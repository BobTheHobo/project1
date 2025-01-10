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
	private Node _parentObj;

	public event EventHandler<_controller.SlowChangedEventArgs> LocalSlowChanged;

	protected void OnLocalSlowChanged(_controller.SlowChangedEventArgs e)
	{
		LocalSlowChanged?.Invoke(this, e);
	}

	public slowableNode(Node parentObj)
	{
		_parentObj = parentObj;
		_controller.GlobalSlowChanged += HandleSlowChange;
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
		// setting to normalexecfactor is basically just turning off slow 
		bool newLocalSlowIsOn = factor != _controller.NormalExecFactor;

		if (newLocalSlowIsOn) 
		{
			// Send out event that slow was changed
			_controller.SlowChangedEventArgs args = new()
			{
				PreviousSlowFactor = LocalSlowFactor,
				CurrentSlowFactor = factor,
				PreviouslyOn = LocalSlowIsOn,
				CurrentlyOn = newLocalSlowIsOn
			};
			OnLocalSlowChanged(args);

			LocalSlowFactor = factor;
			
			LocalSlowIsOn = true;
		}
		else
		{
			LocalSlowmoOff();
		}

	}

	// Local slowmo off (reset this node's speed to normal exec speed)
	public void LocalSlowmoOff()
	{
		// Send out event that slow was toggled
        _controller.SlowChangedEventArgs args = new()
        {
            PreviousSlowFactor = LocalSlowFactor,
            CurrentSlowFactor = _controller.NormalExecFactor,
            PreviouslyOn = LocalSlowIsOn,
            CurrentlyOn = false
        };
		OnLocalSlowChanged(args);

		LocalSlowFactor = _controller.NormalExecFactor;
		LocalSlowIsOn = false;
	}

	// Triggered whenever slow is globally changed 
	// WILL OVERWRITE LOCAL SLOW !!
	private void HandleSlowChange(object sender, _controller.SlowChangedEventArgs e)
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

	/*
     Properly accounts for changes in velocity when changing slowmo
     If this wasn't here, the previous velocity wouldn't change when entering
     slowmo despite gravity and other factors being affected, so 
     your character would have a higher velocity than it should, for example 
     when falling from a non slowmo into a slowmo range 
	*/
    public Vector2 CalcVelocityOnSlowChange(_controller.SlowChangedEventArgs e, Vector2 objVelocity)
    {
		Vector2 newVelocity = objVelocity;

        if (e.CurrentlyOn != e.PreviouslyOn) // slow toggled
        {
            // change current velocity to reflect toggle on
            if (e.PreviouslyOn == false)
            {
                newVelocity *= e.CurrentSlowFactor;
            }
            // Slow toggled off so give previous speed back
            else
            {
                newVelocity /= e.PreviousSlowFactor;
            }
        }
		else if (e.CurrentlyOn && e.PreviouslyOn) // slow continued
		{
			newVelocity *= e.CurrentSlowFactor;
		}

		return newVelocity;
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
