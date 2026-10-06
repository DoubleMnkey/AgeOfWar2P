extends Control

func _ready():
	var sfx_index = AudioServer.get_bus_index("Music")
	var start_value = AudioServer.get_bus_volume_db(sfx_index)
	%MusicSlider.value = db_to_linear(start_value)
	
	var sfx_index2 = AudioServer.get_bus_index("sfx")
	var start_value2 = AudioServer.get_bus_volume_db(sfx_index2)
	%SFXSlider.value = db_to_linear(start_value2)
	
	if get_window().mode == Window.MODE_FULLSCREEN:
		%FullscreenSwitch.button_pressed = true
	else:
		%FullscreenSwitch.button_pressed = false

func _on_button_pressed():
	save_config()
	
	get_tree().change_scene_to_file("res://scenes/main_menu.tscn")

func save_config():
	var config = GlobalVariables.config
	config.set_value("Audio", "music", %MusicSlider.value)
	config.set_value("Audio", "sfx", %SFXSlider.value)
	config.set_value("Video", "fullscreen", %FullscreenSwitch.button_pressed)
	config.save("user://options.cfg")

func _on_slider_value_changed(value: float, bus: StringName) -> void:
	var bus_index = AudioServer.get_bus_index(bus)
	AudioServer.set_bus_volume_db(bus_index, linear_to_db(value))


func _on_fullscreen_toggled(toggled_on: bool) -> void:
	GlobalVariables.update_fullscreen(toggled_on)
