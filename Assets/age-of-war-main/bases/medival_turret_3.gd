extends turret


# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 120
	projectile_speed = 1200
	spawn_projectile_frame = 6
	play_audio_frame = 3
	projectile_texture = load("res://age of war sprites/bases/medival/turret_3/medival_turret_3_projectile.png")
	spawn_projectile_offspring = true
	super()
	play_audio_repeated = true
	offspring_texture = load("res://age of war sprites/bases/medival/turret_3/medival_turret_3_projectile_offspring.png")
	spawn_explosion_effect = true


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
