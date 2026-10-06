extends range_unit


# Called when the node enters the scene tree for the first time.
func _ready():
	health = 350
	damage = 50
	move_speed = 50
	money_die_reward = 2600
	
	sprite_die_position = Vector2(30, -54)
	sprite_idle_position = Vector2(8, -63)
	sprite_idle_attack_position = Vector2(47, -63)
	sprite_melee_attack_position = Vector2(47, -63)
	sprite_walk_position = Vector2(7, -63)
	sprite_walk_attack_position = Vector2(41, -62)
	
	super()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)
	
func idle_attack_state():
	super()
	if $AnimatedSprite2D.frame == 4:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 3:
		$sfx/range_sfx.play()

func melee_attack_state():
	super()
	if $AnimatedSprite2D.frame == 4:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 3:
		$sfx/range_sfx.play()

func walk_attack_state(delta):
	super(delta)
	if $AnimatedSprite2D.frame == 4:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 3:
		$sfx/range_sfx.play()
