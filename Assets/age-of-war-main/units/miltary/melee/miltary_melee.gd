extends melee_unit


# Called when the node enters the scene tree for the first time.
func _ready():
	health = 500
	damage = 80
	move_speed = 50
	
	money_die_reward = 1950
	
	sprite_walk_position = Vector2(0, -64)
	sprite_idle_position = Vector2(0, -59)
	sprite_die_position = Vector2(25, -57)
	sprite_attack_position = Vector2(9, -59)
	
	super()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)


func attack_state():
	if melee_ray_cast.is_colliding() == false:
		change_state(state.walk)
		stop_all_sfx()
		return
	
	if animated_sprite.frame == 3:
		$sfx/stab_02.play()
		# whack_sfx.pitch_scale = randf_range(0.9, 1.1)
	if animated_sprite.frame == 9:
		do_damage(melee_ray_cast.get_collider())
