using Godot;
using System;
using System.Diagnostics;

// NOTE in terms of how I know something is an enemy, I'm doing it in two ways right now. 1: The enemy.cs script has a method IsEnemy which you can check for 2: The main global class is carrying a list of enemies, which should be added to whenever an enemy is instantiated

// Controls everything combat related, including the combat UI
[GlobalClass]
public partial class Combat : Node2D
{
    // Defines singleton instance of Combat
    public static Combat Instance { get; private set; }
    private Node2D _player; // current player
    private Node2D _targetedBody = null; // body that the player is currently targeting
    private bool _inEnemyRange = false; // whether or not player is in range of an enemy
    private bool _lockedOn = false; // whether or not player is locked on to a target
    private Random _rng = new Random(); // Random num generator

    // Lockon crosshairs ui
    private Node lockOnUi = GD.Load<PackedScene>("res://Scenes/UI/lockon_ui.tscn").Instantiate();

    private float _mouseLockOnDistance = 700; // Determines how far away something can be from mouse to be locked onto. 
    // NOTE: this distance is squared

    public enum AttackType // Defines the attack type
    {
        None = 0,
        Light = 1,
        Heavy = 2,
        Special = 3
    }

    System.Collections.ArrayList _enemiesInRange; // Holds enemies that are in range of player

    private bool _enemyInRange = false; // if any enemy is in player attack range
    private bool _enemyInRangeIndicator = false; // indicates that an enemy is in range
    
    // Returns string of an attack type
    // Static because you don't need an instance
    public static string GetAttackString(AttackType attackType)
    {
        switch(attackType)
        {
            case AttackType.None:
            {
                return "None";
            }
            case AttackType.Light:
            {
                return "Light";
            }
            case AttackType.Heavy:
            {
                return "Paper";
            }
            case AttackType.Special:
            {
                return "Special";
            }
            default: // Catch all, shouldn't be triggered?
            {
                return "Undefined";
            }
        } 
    }
    
    // Generates an attack sequence given a number of attacks
    // Returns an array of AttackTypes
    // Can't be static because we need to reference the same RNG instance?
    public AttackType[] GenerateAttackSequence(int numAttacks)
    {
        AttackType[] attackSequence = new Combat.AttackType[numAttacks];
        for (int i = 0; i < numAttacks; i++)
        {
            attackSequence[i] = (AttackType)_rng.Next(1, 4); // Generate random attacks between 1 and 3
        }
        GD.Print("Attack sequence generated:", GetAttackSequenceString(attackSequence));
        return attackSequence;
    }

    // Gets a string of attack types concatenated
    // Static because we don't need to reference a class instance
    public static string GetAttackSequenceString(AttackType[] attacks)
    {
        string sequence = "";
        for (int i = 0; i < attacks.Length; i++)
        {
            sequence += GetAttackString(attacks[i]);
        }
        return sequence;
    }

    private void ConnectToSignals()
    {
        SignalBus.Instance.EnemyEnteredAttackRange += OnEnemyEnteredAttackRange;
        SignalBus.Instance.EnemyLeftAttackRange += OnEnemyLeftAttackRange;
    }

    private void OnEnemyEnteredAttackRange(Node2D body)
    {
        AddEnemyInRange(body);
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.ShowEnemyInRangeUI, true);
    }

    private void OnEnemyLeftAttackRange(Node2D body)
    {
        RemoveEnemyInRange(body);
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.ShowEnemyInRangeUI, false);
    }

    // Adds enemy to enemy in range array
    private void AddEnemyInRange(Node2D enemy)
    {
        _enemiesInRange.Add(enemy);
        _enemyInRange = true;
        GD.Print("Enemy entered player atrange " + enemy);
    }

    // Checks if enemy is in enemy array and removes if they are
    private void RemoveEnemyInRange(Node2D enemy)
    {
        _enemiesInRange.Remove(enemy); // No exception will be thrown if enemy can't be found so watch out
        _enemyInRange = IsAnyEnemyInRange();
    }

    // Returns if the enemies in range array is empty (no enemies in attack range)
    private bool IsAnyEnemyInRange()
    {
        return _enemiesInRange.Count != 0;
    }

    // Sets player's current target
    private void SetTarget(Node2D target)
    {
        _targetedBody = target;
    }

    // Clears player's current target
    private void ClearTarget()
    {
        _targetedBody = null;
    }

    // Returns nearest enemy to given coordinates, if no enemy found will return null
    // Will find closest enemy no matter the distance so use carefully
    private enemy FindNearestEnemy(Vector2 pos)
    {
        enemy closestEnemy = null;
        float closestDist = 9999999; // really large number

        // TODO: might have to find out a way to prioritize if multiple enemies are the same distance away for now it just takes whichever is found first
        foreach (enemy en in Main.GetEnemies())
        {
            float dist = pos.DistanceSquaredTo(en.GlobalPosition);
            if (dist < closestDist) {
                closestEnemy = en;
                closestDist = dist;
            }
        }
        return closestEnemy;
    }

    //if mouseLock is used will find the target within a certain distance
    //  to the mouse and lock onto that,
    //else will just use whatever is nearest to the player
    //TODO: Might want to have ability to lock onto things other than enemies
    private void HandleLockOnToEnemy(Boolean mouseLock)
    {
        enemy targetedEnemy = null;

        if (mouseLock) // Using mouselock
        {
            // Get mouse position method works b/c Combat inherits from Node2D (inheriting JUST NODE WON'T HAVE THIS METHOD)

            // FIXME: GetGlobalMousePosition will get the position of cursor EVEN IF IT'S NOT ON THE GAME SCREEN, might have to implement custom cursor and disable default?
            Vector2 mousePos = GetGlobalMousePosition();
            targetedEnemy = FindNearestEnemy(mousePos);

            // Limits lock on range to only inside certain distance from mouse
            float distToMouse = targetedEnemy.GlobalPosition.DistanceSquaredTo(mousePos);
            GD.Print("Distance from target to mouse is " + distToMouse.ToString());
            //FIXME: The distance from enemy is calculated from the sprite origin, which is at the bottom. This means that sprites have a larger detection range at the bottom than the top, which might lead to some problems... for now i think it's ok though... could be fixed by making a custom cursor that has it's own collision shape? or maybe just have a target indicator that visually snaps to different targets so you know what you're locking to
            if (distToMouse > _mouseLockOnDistance)
            {
                targetedEnemy = null;
            }
        }
        else // Default lockon (by distance to player)
        {
            Vector2 playerPos = Main._player.GlobalPosition;
            targetedEnemy = FindNearestEnemy(playerPos);

            // Limited to enemies inside player's detection range, might pose some weird issues later esp if mouse lockon range is essentially unlimited (based on mouse position) whereas player lockon is a smaller distance?
            if (!IsAnyEnemyInRange())
            {
                targetedEnemy = null;
            }
        }

        // LockOnToTarget will handle if enemy was null
        LockOnToTarget(targetedEnemy);
    }
    
    // Locks on to a given target
    private void LockOnToTarget(Node2D target)
    {
        if (target == null) {
            LockOff();
            GD.Print("No target to lock onto");
        } else {
            _lockedOn = true;
            SetTarget(target);
            attachLockOnUi(target);
            GD.Print("Player locked on to: ", target.Name);
        }
    }

    // Turns off lock
    private void LockOff()
    {
        _lockedOn = false;
        ClearTarget(); // Resets targeted node
        removeLockOnUi();
    }

    // Attempts to remove lockonui from its parent
    private void removeLockOnUi()
    {
        if (lockOnUi.GetParentOrNull<Node>() != null)
        {
            lockOnUi.GetParent().RemoveChild(lockOnUi);
        }
    }

    // Attaches lockonui as a child of the target
    private void attachLockOnUi(Node2D target) {
        // Remove from existing parent, if any
        removeLockOnUi();
        CollisionShape2D targetColShape = target.GetNodeOrNull<CollisionShape2D>("SpriteCollisionShape");
        // If target has a collision shape add it to that b/c it'll be more centered than using the actual object origin
        if (targetColShape != null)
        {
            targetColShape.AddChild(lockOnUi);
        }
        else
        {
            target.AddChild(lockOnUi);
        }
    }

    // Not sure if I should put this here or in the actual player class
    //  but we can always move later
    //  Also not sure if it has input lag
    public void CombatInputHandler(InputEvent @event)
    {
        if (@event.IsActionPressed("LockOn"))
        {
            // Just have mouse handle the locking in for now
            HandleLockOnToEnemy(true);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this; // Needed to link this instance as singleton instance
        _enemiesInRange = new System.Collections.ArrayList();
        ConnectToSignals();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
