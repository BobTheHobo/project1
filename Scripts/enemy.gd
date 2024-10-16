extends CharacterBody2D

var speed = 35
var player_chase = false
var player = null # target player
var player_in_attack_range = false # if player is in attack range
var direction = 1 # move to right by default
var rng = RandomNumberGenerator.new()
var attackSequence = []

func attackSequenceString() -> String:
	var sequence: String = ""
	for i:int in range(attackSequence.size()):
		sequence += str(attackSequence[i])
	return sequence

func generateAttackSequence(numAttacks: int):
	for i in range(numAttacks):
		var attack = rng.randi_range(0,2)
		attackSequence.push_back(attack)
	print("Attack sequence generated: %s" % attackSequenceString())
	
func handle_animations():
	if player_in_attack_range:
		$AnimatedSprite2D.play("attack")
	else:
		$AnimatedSprite2D.play("idle")

func move_enemy():
	# velocity.x = speed
	
	if not is_on_floor():
		velocity += get_gravity()
		
func _physics_process(delta: float) -> void:
	handle_animations()
	move_enemy()
	
	if player_chase:
		position.x += (player.position.x - position.x)/speed
		
		if(player.position.x - position.x) < 0:
			$AnimatedSprite2D.flip_h = true
		else:
			$AnimatedSprite2D.flip_h = false
		
	move_and_slide()

func _on_detection_area_body_entered(body: Node2D) -> void:
	player = body
	player_chase = true

func _on_detection_area_body_exited(body: Node2D) -> void:
	player = null
	player_chase = false

func _on_attack_range_body_entered(body: Node2D) -> void:
	if (body == player):
		player_in_attack_range = true
	
func _on_attack_range_body_exited(body: Node2D) -> void:
	if (body == player):
		player_in_attack_range = false

func _ready() -> void:
	generateAttackSequence(6)

	# Method to signifiy that this is an enemy, DON'T DELETE
func enemy():
	pass
