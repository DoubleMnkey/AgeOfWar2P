extends melee_unit


# Called when the node enters the scene tree for the first time.
func _ready():
	health = 1100
	damage = 160
	move_speed = 50
	money_die_reward = 6500
	
	sprite_walk_position = Vector2(-33, -69)
	sprite_idle_position = Vector2(-33, -69)
	sprite_die_position = Vector2(-14, -67)
	sprite_attack_position = Vector2(9, -91)
	
	super()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)

func attack_state():
	if melee_ray_cast.is_colliding() == false:
		change_state(state.walk)
		stop_all_sfx()
		return
	
	
	if animated_sprite.frame == 18:
		do_damage(melee_ray_cast.get_collider())
