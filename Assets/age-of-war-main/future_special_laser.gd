extends Area2D


var is_player_owned: bool
var damage : int = 600

func _ready():
	is_player_owned = true
	self.connect("body_entered", _on_body_entered)
	$AudioStreamPlayer2D.bus = &'sfx'
	$AudioStreamPlayer2D.play()


func _on_body_entered(body):
	if body is melee_unit or body is range_unit:
		if body.is_player_owned != self.is_player_owned:
			body.take_damage(damage)
			$CollisionShape2D.set_deferred("disabled", true)


func _on_audio_stream_player_2d_finished():
	self.queue_free()
