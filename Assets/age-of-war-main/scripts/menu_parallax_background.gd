extends ParallaxBackground

@export var use_x: bool = true
@export var use_y: bool = true
@export var parallax_x_strength: float = 10
@export var parallax_y_strength: float = 4

func _ready() -> void:
	update_parallax()

func _process(delta) -> void:
	update_parallax()

func update_parallax() -> void:
	var mouse_position = get_viewport().get_mouse_position() * Vector2(parallax_x_strength, parallax_y_strength) / 100.0
	if use_x:
		scroll_offset.x = -mouse_position.x
	if use_y:
		scroll_offset.y = -mouse_position.y
