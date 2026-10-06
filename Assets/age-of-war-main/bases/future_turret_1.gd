extends turret


# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 100
	projectile_speed = 1200
	spawn_projectile_frame = 2
	play_audio_frame = 1
	projectile_texture = load("res://age of war sprites/bases/future/turret_1/future_turret_1_projectile.png")
	super()
	no_rotation = true


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
