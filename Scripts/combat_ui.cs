using Godot;
using System;

public partial class combat_ui : CanvasLayer
{
    private Label _enemyInRangeUI;

    // Called when the node enters the scene tree for the first time.
    public void AssignElements()
    {
        _enemyInRangeUI = GetNode<Label>("EnemyInRangeLabel");
    }

    public void ConnectToSignals()
    {
        SignalBus.Instance.ShowEnemyInRangeUI += ShowEnemyInRangeUIt;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public void _ready()
    {
        // Ensure that canvaslayer doesn't move and shift ui items offscreen
        this.FollowViewportEnabled = false;

        AssignElements();
        ConnectToSignals();

        _enemyInRangeUI.Visible = false;
    }

    public void ShowEnemyInRangeUIt(bool visible)
    {
        if (visible)
        {
            _enemyInRangeUI.Visible = true;
        }
        else
        {
            _enemyInRangeUI.Visible = false;
        }
    }
}
