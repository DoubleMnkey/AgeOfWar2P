extends Node2D


# Credit: https://totuslotus.itch.io/pixel-coins

# Called when the node enters the scene tree for the first time.
func _ready():
	$Timer.start(4.0)
	$Timer2.start(1.0)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	self.global_position.y += -20 * delta


func _on_timer_timeout():
	self.queue_free()


func _on_timer_2_timeout():
	var tween = create_tween()
	tween.tween_property(self, "modulate", Color(1,1,1,0), 1.0)
	tween.play()
