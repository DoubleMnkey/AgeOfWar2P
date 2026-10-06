extends Area2D

var damage: int = 10
@export var direction : Vector2 = Vector2(0,0)
var speed: float = 10
@export var time_to_die : float = 5.0
var is_player_owned: bool = true
var acceleration : Vector2 = Vector2(0, 20)

var spawn_explosion_effect


# Called when the node enters the scene tree for the first time.
func _ready():
	$Timer.start(time_to_die)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	position += direction * speed * delta
	direction += acceleration * delta
	if direction.y > 0:
		collision_mask = 1


func _on_timer_timeout():
	self.queue_free()


func _on_body_entered(body):
	if body is melee_unit or body is range_unit:
		if body.is_player_owned != self.is_player_owned:
			body.take_damage(damage)
			if spawn_explosion_effect == true:
				var explosion = load("res://effects/explosion_effect.tscn").instantiate()
				explosion.global_position = self.global_position
				explosion.global_position.y -= 32
				explosion.scale = Vector2(0.125, 0.125)
				explosion.get_node("AudioStreamPlayer2D").volume_db = -10
				get_node("/root/main_game").add_child(explosion)
			self.queue_free()
	elif body.name == "floor":
		if spawn_explosion_effect == true:
			var explosion = load("res://effects/explosion_effect.tscn").instantiate()
			explosion.global_position = self.global_position
			explosion.global_position.y -= 48 - randi_range(0, 32)
			explosion.scale = Vector2(0.125, 0.125)
			explosion.get_node("AudioStreamPlayer2D").volume_db = -10
			get_node("/root/main_game").add_child(explosion)
			
			var fire = load("res://effects/fire_effect.tscn").instantiate()
			fire.global_position = self.global_position
			fire.global_position.y -= 32 - randi_range(0, 32)
			fire.scale = Vector2(0.06, 0.06)
			get_node("/root/main_game").add_child(fire)
		self.queue_free()
