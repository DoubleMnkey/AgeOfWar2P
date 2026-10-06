extends turret


# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 120
	projectile_speed = 1200
	spawn_projectile_frame = 6
	play_audio_frame = 3
	projectile_texture = load("res://age of war sprites/bases/medival/turret_2/medival_turret_2_projectile.png")
	super()
	# offset = Vector2(64, 0)
	play_audio_repeated = true


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
