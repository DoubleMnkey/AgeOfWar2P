extends Node2D

var audioStreamPlayer : AudioStreamPlayer

func _ready():
	audioStreamPlayer = get_node("AudioStreamPlayer")
	if get_node("/root").has_node("main_game"):
		audioStreamPlayer.play()
	audioStreamPlayer.bus = &'Music'
