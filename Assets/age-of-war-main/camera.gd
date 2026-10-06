extends Camera2D

@export var scroll_speed = 400
@export var left_most_position = 500
@export var right_most_position = 1350

@export var randomStrength: float = 10.0
@export var shakeFade: float = 1.0

var velocity

var rng = RandomNumberGenerator.new()
var shake_strength: float = 0.0

func apply_shake():
	shake_strength = randomStrength

func randomOffset() -> Vector2:
	return Vector2(rng.randf_range(-shake_strength, shake_strength), rng.randf_range(-shake_strength, shake_strength))

func _ready():
	self.limit_right = right_most_position + 576 
	velocity = Vector2.ZERO

func _process(delta):
	if velocity != Vector2.ZERO:
		var strength: float = 1.0
		position.x += int(velocity.x * delta * scroll_speed)
	position.x = clamp(position.x, left_most_position, right_most_position)
	
	if shake_strength > 0:
		shake_strength = lerpf(shake_strength, 0.0, shakeFade * delta)
		
		offset = randomOffset()

func camera_shake(duration: float, intensity: float):
	pass

func stop_camera_moving():
	velocity = Vector2.ZERO

func set_camera_movement_velocity(vel: float):
	velocity = Vector2(vel, 0.0)
