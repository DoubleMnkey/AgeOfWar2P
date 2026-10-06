extends turret


# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 40
	projectile_speed = 500
	spawn_projectile_frame = 19
	play_audio_frame = 12
	projectile_texture = load("res://age of war sprites/bases/knight/turret_1/knight_turret_1_projectile.png")
	super()
	offset = Vector2(0, -96)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
