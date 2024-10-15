extends CharacterBody2D

var health = 100
var player_alive = true

const SPEED = 200.0
const JUMP_VELOCITY = -300.0

var direction_facing = 1 # 1 is right, -1 is le ft

# Method specifies that this is a player DO NOT REMOVE
func player():
	pass
	
func _physics_process(delta: float) -> void:
	# Add the gravity.
	if not is_on_floor():
		velocity += get_gravity() * delta

	# Handle jump.
	if Input.is_action_just_pressed("ui_accept") and is_on_floor():
		velocity.y = JUMP_VELOCITY

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var direction := Input.get_axis("ui_left", "ui_right")
	if (direction != 0):
		direction_facing = direction
	
	# flip player model if going left
	if(direction_facing == -1):
		$AnimatedSprite2D.flip_h = true
	else:
		$AnimatedSprite2D.flip_h = false
	
	if direction:
		velocity.x = direction * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)

	move_and_slide()

func _on_attack_range_body_entered(body: Node2D) -> void:
	if body.has_method("enemy"):
		Combat.addEnemyInRange(body)

func _on_attack_range_body_exited(body: Node2D) -> void:
	if body.has_method("enemy"):
		Combat.removeEnemyInRange(body)
