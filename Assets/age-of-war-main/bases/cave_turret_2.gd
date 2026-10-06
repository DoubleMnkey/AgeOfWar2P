extends turret


# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 2
	projectile_speed = 300
	spawn_projectile_frame = 3
	play_audio_frame = 2
	projectile_texture = load("res://age of war sprites/bases/cave/turret_2/cave_turret_2_projectile.png")
	projectile_rotation = true
	super()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
