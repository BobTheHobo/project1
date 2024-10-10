extends CharacterBody2D

var speed = 35
var player_chase = false
var player = null # target player
var direction = 1 # move to right by default

func move_enemy():
	# velocity.x = speed
	
	if not is_on_floor():
		velocity += get_gravity()

func _physics_process(delta: float) -> void:
	move_enemy()
	
	$AnimatedSprite2D.play("idle")
		
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
	
func enemy():
	pass
