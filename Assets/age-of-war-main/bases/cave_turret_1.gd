extends turret

# Called when the node enters the scene tree for the first time.
func _ready():
	projectile_damge = 15
	projectile_speed = 400
	spawn_projectile_frame = 5
	play_audio_frame = 1
	projectile_texture = load("res://age of war sprites/bases/cave/turret_1/cave_turret_1_projectile_01.png")
	super()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
