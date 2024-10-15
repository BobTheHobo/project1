# added to global scripts
# EIR = enemy in range
extends Node

var player # current player
var targeted_body = null # body that the player is currently targeting
var in_enemy_range = false # whether or not player is in range of an enemy

var enemies_in_range = [] # holds enemies that are in range of player
var enemy_in_range = false # if any enemy is in player attack range

var enemy_in_range_indicator = false # indicates that an enemy is in range

# Adds enemy to enemy in range array
func addEnemyInRange(enemy: Node2D) -> void:
	enemies_in_range.append(enemy)
	print("Enemy entered player atrange %s" % enemy)
	enemy_in_range = true

# Checks if enemy is in enemy array and removes if they are
func removeEnemyInRange(enemy: Node2D) -> void:
	var i = enemies_in_range.find(enemy) 
	if(i != -1):
		enemies_in_range.remove_at(i)
		print("Enemy exited player atrange %s" % enemy)
	else:
		print("Could not find enemy in range: %s" % enemy)
	enemy_in_range = isAnyEnemyInRange()

# Returns if the enemies in range array is empty (no enemies in attack range)
func isAnyEnemyInRange() -> bool:
	return !enemies_in_range.is_empty()

# Sets player's current target
func setTarget(target: Node2D) -> void:
	targeted_body = target
	
# Clears player's current target
func clearTarget() -> void:
	targeted_body = null

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass
		
# Does something to indicate that an enemy is in attack range
func indicateEnemyInRange() -> void:
	if (isAnyEnemyInRange()):
		enemy_in_range_indicator = true
	else:
		enemy_in_range_indicator = false
		
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	indicateEnemyInRange()
