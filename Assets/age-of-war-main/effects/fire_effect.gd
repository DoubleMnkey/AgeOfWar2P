extends AnimatedSprite2D


# Called when the node enters the scene tree for the first time.
func _ready():
	$AudioStreamPlayer2D.pitch_scale = randf_range(0.9, 1.1)
	self.z_index = -1
	$AudioStreamPlayer2D.play()
	$AudioStreamPlayer2D.bus = &'sfx'


func _on_audio_stream_player_2d_finished():
	self.queue_free()
