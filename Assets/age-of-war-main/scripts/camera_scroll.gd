extends ColorRect

@export var camera: Camera2D
@export var inverted: bool = false

var velocity: float = 0.0
var mouse_over: bool = false

func _ready() -> void:
	if camera == null:
		printerr("[", name, "] no camera has been assigned")
	
	self_modulate.a = 0.0
		
	connect("mouse_entered", _on_mouse_entered)
	connect("mouse_exited", _on_mouse_exited)
	connect("gui_input", _on_gui_input)

func _process(delta: float) -> void:
	if not mouse_over:
		return
	
	camera.set_camera_movement_velocity(-velocity if inverted else velocity)

func _on_mouse_entered() -> void:
	mouse_over = true

func _on_mouse_exited() -> void:
	mouse_over = false
	camera.stop_camera_moving()

func _on_gui_input(event: InputEvent) -> void:
	if event is not InputEventMouseMotion:
		return
	
	velocity = clampf(event.position.x / size.x, 0.0, 1.0)
	velocity = 1.0 - velocity if inverted else velocity
