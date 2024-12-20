using Godot;
using System;
using System.Diagnostics;

// Added to global scripts
public partial class Main : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        IsDebug();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        CheckForQuit();
    }

    private void CheckForQuit()
    {
        if (Input.IsActionJustPressed("Quit"))
        {
            GetTree().Quit(); // Quit the game
            GD.Print("Escape pressed, quitting the game");
        }
    }

    private void IsDebug()
    {
        if (OS.IsDebugBuild())
        {
            GD.Print("Running in debug build");
        }
        else
        {
            GD.Print("Running in release build");
        }

    }

}
