extends Area2D

var damage: int = 500
@export var direction : Vector2 = Vector2(0,1)
var speed: float = 10
@export var time_to_die : float = 5.0
var is_player_owned: bool = true
var acceleration : Vector2 = Vector2(0, 20)
var random_rotation_speed

# Called when the node enters the scene tree for the first time.
func _ready():
	$Timer.start(time_to_die)
	random_rotation_speed = randf_range(-100.0, 100.0)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	position += direction * speed * delta
	direction += acceleration * delta
	if direction.y > 0:
		collision_mask = 1
	self.rotation_degrees += random_rotation_speed * delta


func _on_timer_timeout():
	self.queue_free()


func _on_body_entered(body):
	if body is melee_unit or body is range_unit:
		if body.is_player_owned != self.is_player_owned:
			body.take_damage(damage)
			spawn_explosion_effect()
			self.queue_free()
	elif body.name == "floor":
		spawn_explosion_effect()
		self.queue_free()

func spawn_explosion_effect():
	var explosion = load("res://effects/explosion_effect.tscn").instantiate()
	explosion.global_position = self.global_position
	explosion.global_position.y -= 64
	get_parent().add_child(explosion)
