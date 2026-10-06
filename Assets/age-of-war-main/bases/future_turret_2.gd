extends turret


# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 50
	projectile_speed = 1200
	spawn_projectile_frame = 2
	play_audio_frame = 1
	projectile_texture = load("res://age of war sprites/bases/future/turret_2/future_turret_2_projectile0001.png")
	super()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
