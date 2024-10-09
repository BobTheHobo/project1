extends CharacterBody2D

const SPEED = 200.0
const JUMP_VELOCITY = -300.0

func _physics_process(delta: float) -> void:
	# Add the gravity.
	if not is_on_floor():
		velocity += get_gravity() * delta

	# Handle jump.
	if Input.is_action_just_pressed("ui_accept") and is_on_floor():
		velocity.y = JUMP_VELOCITY

	# Get the input direction and handle the movement/deceleration.
	var direction := Input.get_axis("ui_left", "ui_right")
	
	# Flip the player sprite based on movement direction
	if direction < 0:
		$AnimatedSprite2D.scale.x = -1  # Face left
	elif direction > 0:
		$AnimatedSprite2D.scale.x = 1   # Face right

	# Set velocity based on movement direction
	if direction:
		velocity.x = direction * SPEED
		$AnimatedSprite2D.play("Run")  # Play run animation when moving
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		$AnimatedSprite2D.play("Idle")  # Play idle animation when not moving

	move_and_slide()
