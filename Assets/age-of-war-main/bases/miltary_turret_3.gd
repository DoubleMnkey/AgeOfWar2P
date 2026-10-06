extends turret


# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 80
	projectile_speed = 1200
	spawn_projectile_frame = 6
	play_audio_frame = 5
	spawn_projectile_frame2 = 23
	play_audio_frame2 = 22
	projectile_texture = load("res://age of war sprites/bases/miltary/turret_1/miltary_turret_1_projectile.png")
	super()
	play_audio_repeated = true


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
