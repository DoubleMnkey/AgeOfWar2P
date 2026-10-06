extends Area2D

var damage: int = 10
@export var direction : Vector2 = Vector2(0,0)
var speed: float = 10
@export var time_to_die : float = 5.0
var is_player_owned: bool = true
var acceleration : Vector2 = Vector2(0, 20)

# Called when the node enters the scene tree for the first time.
func _ready():
	$Timer.start(time_to_die)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_body_entered(body):
	if body is melee_unit or body is range_unit:
		if body.is_player_owned != self.is_player_owned:
			body.take_damage(damage)
			print("spawn explosion effect")
			self.queue_free()
	elif body.name == "floor":
		print("spawn explosion effect")
		self.queue_free()
