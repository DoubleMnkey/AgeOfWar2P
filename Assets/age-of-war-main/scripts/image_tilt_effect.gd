extends TextureRect

@export var enable_eyecandy: bool = true
@export_category("Tilt")
@export var tilt_strength: float = 0.5
@export var tilt_speed: float = 2.0
@export_category("Zoom")
@export var zoom_start: Vector2 = Vector2(0.5, 0.5)
@export var zoom_end: Vector2 = Vector2(0.75, 0.75)
@export var zoom_speed: float = 2.0

var target_uv: Vector2 = Vector2(0.5, 0.5)
var mouse_over: bool = false

func _process(_delta):
	if not enable_eyecandy:
		material.set_shader_parameter("shimmer_strength", 0.0)
		return
	
	if not mouse_over:
		target_uv = Vector2(0.5, 0.5)
	
	else:
		var local_mouse = get_local_mouse_position()
		target_uv = Vector2(
			1.0 - clamp(local_mouse.x / size.x, 0.0, 1.0),
			1.0 - clamp(local_mouse.y / size.y, 0.0, 1.0)
		)
	material.set_shader_parameter("mouse_pos", lerp(material.get_shader_parameter("mouse_pos"), target_uv, tilt_speed * _delta))
	material.set_shader_parameter("shimmer_strength", lerpf(material.get_shader_parameter("shimmer_strength"), 0.25 if mouse_over else 0.0, zoom_speed * _delta))
	material.set_shader_parameter("tilt_strength", tilt_strength)
	material.set_shader_parameter("card_size", size)
	scale = lerp(scale, zoom_end if mouse_over else zoom_start, zoom_speed * _delta)

func _on_mouse_entered() -> void:
	mouse_over = true

func _on_mouse_exited() -> void:
	mouse_over = false
