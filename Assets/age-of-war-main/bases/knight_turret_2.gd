extends turret


# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 60
	projectile_speed = 500
	spawn_projectile_frame = 19
	play_audio_frame = 12
	projectile_texture = load("res://age of war sprites/bases/knight/turret_2/knight_turret_2_projectile.png")
	spawn_projectile_offspring = true
	super()
	offset = Vector2(0, -96)
	spawn_explosion_effect = true
	no_rotation = true
	projectile_rotation = true
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
