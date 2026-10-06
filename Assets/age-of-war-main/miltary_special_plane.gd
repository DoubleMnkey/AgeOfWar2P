extends Node2D

var direction
var speed

var timer : Timer

# Called when the node enters the scene tree for the first time.
func _ready():
	direction = Vector2.RIGHT
	speed = 200
	timer = get_node("Timer")
	timer.paused = true
	timer.wait_time = 1.5
	$AudioStreamPlayer2D.bus = &'sfx'
	$AudioStreamPlayer2D.play()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	position.x +=  speed * delta
	
	if position.x > 2400:
		self.queue_free()
	elif position.x > 1500:
		timer.stop()
	elif position.x > 200 and timer.is_paused() == true:
		timer.start(0.5)
		timer.paused = false




func spawn_bomb():
	var bomb = load("res://projectile_miltary_special_bomb.tscn").instantiate()
	bomb.global_position = self.global_position
	bomb.global_position.y += 16
	bomb.is_player_owned = true
	get_node("/root/main_game").add_child(bomb)


func _on_bomb_timer_timeout():
	spawn_bomb()
