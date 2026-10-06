extends Sprite2D

@export var speed: float = 8.0

func _ready() -> void:
	self_modulate.a = 0.0

func _process(delta: float) -> void:
	self_modulate.a = lerpf(self_modulate.a, 0.0, speed * delta)

func flash():
	self_modulate.a = 1.0
