using Godot;
using System;
using System.Collections.Generic;

// Added to global scripts
public partial class Main : Node
{
    // Using a dynamic list to hold enemies in scene
    private static List<enemy> _enemies = new List<enemy>();

    // Holds a reference to player with read only access
    public static player _player {get; set;}

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

    // Adds enemy to global list for usage in other scripts
    public static void AddEnemy(enemy enmy)
    {
        _enemies.Add(enmy);
        GD.Print("Enemy added to global list: " + enmy);
    }

    // Returns list of enemeies
    public static List<enemy> GetEnemies()
    {
        return _enemies;
    }

    // Prints out list of current enemies in list
    public static void printEnemies()
    {
        GD.Print(_enemies.Count.ToString() + " enemies in list:");
        foreach (enemy en in _enemies)
        {
            GD.Print("  " + en.Name + ": " + en.ToString());    
        }
    }
}
