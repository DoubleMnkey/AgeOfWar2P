extends Control

func _on_normal_pressed():
	MusicManager.audioStreamPlayer.play()
	GlobalVariables.current_difficulty = GlobalVariables.difficulty.normal
	get_tree().change_scene_to_file("res://main_game.tscn")

func _on_hard_pressed():
	MusicManager.audioStreamPlayer.play()
	GlobalVariables.current_difficulty = GlobalVariables.difficulty.hard
	get_tree().change_scene_to_file("res://main_game.tscn")

func _on_impossible_pressed():
	MusicManager.audioStreamPlayer.play()
	GlobalVariables.current_difficulty = GlobalVariables.difficulty.impossible
	get_tree().change_scene_to_file("res://main_game.tscn")

func _on_return_pressed():
	get_tree().change_scene_to_file("res://scenes/main_menu.tscn")
