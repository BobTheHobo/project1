extends CanvasLayer

var enemyInRangeUI

func assignElements() -> void:
	enemyInRangeUI = $EnemyInRangeLabel

func connectToSignals() -> void:
	SignalBus.showEnemyInRangeUI.connect(_show_enemy_in_range_ui)

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	assignElements()
	connectToSignals()
	
	# by default enemy in range indicator should be hidden
	enemyInRangeUI.visible = false
	
func _show_enemy_in_range_ui(visible: bool) -> void:
	if (visible):
		enemyInRangeUI.visible = true
	else:
		enemyInRangeUI.visible = false

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
