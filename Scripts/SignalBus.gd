extends Node

# define global signals needed here

signal enemyEnteredAttackRange(enemy: Node2D)
signal enemyLeftAttackRange(enemy: Node2D)

# Combat UI signals
signal showEnemyInRangeUI(show: bool)
