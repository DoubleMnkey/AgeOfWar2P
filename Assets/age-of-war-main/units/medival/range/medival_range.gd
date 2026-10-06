extends range_unit


# Called when the node enters the scene tree for the first time.
func _ready():
	health = 130
	damage = 25
	move_speed = 50
	money_die_reward = 520
	
	sprite_die_position = Vector2(-16, -63)
	sprite_idle_position = Vector2(20, -65)
	sprite_idle_attack_position = Vector2(59, -64)
	sprite_melee_attack_position = Vector2(59, -64)
	sprite_walk_position = Vector2(20, -67)
	sprite_walk_attack_position = Vector2(36, -66)
	
	super()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)



func idle_attack_state():
	super()
	if $AnimatedSprite2D.frame == 12:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 11:
		$sfx/range_sfx.play()

func melee_attack_state():
	super()
	if $AnimatedSprite2D.frame == 12:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 11:
		$sfx/range_sfx.play()

func walk_attack_state(delta):
	super(delta)
	if $AnimatedSprite2D.frame == 3:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 2:
		$sfx/range_sfx.play()
