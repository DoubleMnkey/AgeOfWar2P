extends turret


# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 300
	projectile_speed = 500
	spawn_projectile_frame = 17
	play_audio_frame = 99
	projectile_texture = load("res://age of war sprites/bases/knight/turret_3/knight_turret_3_projectile.png")
	super()
	offset = Vector2(185, 0)
	no_rotation = true


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
