extends AnimatedSprite2D


# Called when the node enters the scene tree for the first time.
func _ready():
	
	# self.connect("animation_finished", _on_animation_finished)
	$AudioStreamPlayer2D.play()
	$AudioStreamPlayer2D.connect("finished", _on_audio_finished)
	$AudioStreamPlayer2D.pitch_scale = randf_range(0.9, 1.1)
	$AudioStreamPlayer2D.bus = &'sfx'



func _on_animation_finished():
	self.queue_free()

func _on_audio_finished():
	self.queue_free()
