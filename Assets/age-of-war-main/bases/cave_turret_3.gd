extends turret


# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 40
	projectile_speed = 500
	spawn_projectile_frame = 7
	play_audio_frame = 2
	projectile_texture = load("res://age of war sprites/bases/cave/turret_3/cave_turret_3_projectile.png")
	super()
	offset = Vector2(0, -64)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
