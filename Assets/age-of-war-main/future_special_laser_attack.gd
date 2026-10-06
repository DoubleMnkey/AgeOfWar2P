extends Node2D


var number_of_steps = 20
var i = 0

# Called when the node enters the scene tree for the first time.
func _ready():
	$Timer.start(0.10)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_timer_timeout():
	if i == number_of_steps:
		self.queue_free()
	else:
		i += 1
		var laser = load("res://future_special_laser.tscn").instantiate()
		laser.global_position = Vector2(250 + 64 * i, 570)
		get_node("/root/main_game").add_child(laser)
