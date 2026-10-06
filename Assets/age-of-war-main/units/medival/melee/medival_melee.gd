extends melee_unit


# Called when the node enters the scene tree for the first time.
func _ready():
	health = 240
	damage = 40
	move_speed = 50
	money_die_reward = 260
	
	sprite_walk_position = Vector2(21, -63)
	sprite_idle_position = Vector2(5, -63)
	sprite_die_position = Vector2(-11, -67)
	sprite_attack_position = Vector2(36, -90)
	
	
	super()
	
	multiple_attack_animations = true


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)

func attack_state():
	if melee_ray_cast.is_colliding() == false:
		change_state(state.walk)
		return
	
	if animated_sprite.get_animation() == "attack_1":
		if animated_sprite.frame == 3:
			$sfx/stab_01.play()
		if animated_sprite.frame == 16:
			do_damage($RayCast2D.get_collider())
	else:
		# attack_2 is playing
		if animated_sprite.frame == 10:
			$sfx/whoosh_02.play()
			$sfx/sword_clash_01.play()
		if animated_sprite.frame == 23:
			do_damage($RayCast2D.get_collider())
	pass
