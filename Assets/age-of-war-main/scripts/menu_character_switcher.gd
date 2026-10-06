extends Control

@export var visible_time: float = 2.0
@export var fade_speed: float = 16.0

const ALPHA_VISIBLE_THRESHOLD := 0.99
const ALPHA_INVISIBLE_THRESHOLD := 0.01

var current_character_index: int = 0
var is_fading_in: bool = true
var is_fading_out: bool = false
var wait_time: float = 0.0

func _ready() -> void:
	for child in get_children():
		child.visible = true
		child.self_modulate.a = 0.0

func _process(delta: float) -> void:
	var character := get_child_or_null(current_character_index)
	if character == null:
		return

	var current_alpha: float = character.self_modulate.a

	if is_fading_in:
		character.self_modulate.a = lerpf(current_alpha, 1.0, delta * fade_speed)
		if character.self_modulate.a >= ALPHA_VISIBLE_THRESHOLD:
			character.self_modulate.a = 1.0
			is_fading_in = false
	elif is_fading_out:
		character.self_modulate.a = lerpf(current_alpha, 0.0, delta * fade_speed)
		if character.self_modulate.a <= ALPHA_INVISIBLE_THRESHOLD:
			character.self_modulate.a = 0.0
			is_fading_out = false
			is_fading_in = true
			wait_time = 0.0
			current_character_index = (current_character_index + 1) % get_child_count()
	else:
		wait_time += delta
		if wait_time >= visible_time:
			is_fading_out = true

func get_child_or_null(index: int) -> Node:
	return get_child(index) if index < get_child_count() else null
