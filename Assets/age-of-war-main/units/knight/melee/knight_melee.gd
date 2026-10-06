extends melee_unit



# Called when the node enters the scene tree for the first time.
func _ready():
	health = 120
	damage = 20
	move_speed = 50
	money_die_reward = 75
	
	sprite_walk_position = Vector2(26, -51)
	sprite_idle_position = Vector2(16, -51)
	sprite_die_position = Vector2(40, -45)
	sprite_attack_position = Vector2(34, -76)
	
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
		if animated_sprite.frame == 18:
			$sfx/whoosh_02.play()
			$sfx/clash_02.play()
		if animated_sprite.frame == 28:
			do_damage($RayCast2D.get_collider())
	else:
		# attack_2 is playing
		if animated_sprite.frame == 20:
			$sfx/stab_02.play()
		if animated_sprite.frame == 23:
			do_damage($RayCast2D.get_collider())
	pass
