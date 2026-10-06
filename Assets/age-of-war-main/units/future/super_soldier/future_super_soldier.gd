extends range_unit


# Called when the node enters the scene tree for the first time.
func _ready():
	health = 20000
	damage = 300
	move_speed = 50
	money_die_reward = 0
	
	sprite_die_position = Vector2(-22, -53)
	sprite_idle_position = Vector2(4, -63)
	sprite_idle_attack_position = Vector2(31, -62)
	sprite_melee_attack_position = Vector2(13, -71)
	sprite_walk_position = Vector2(3, -61)
	sprite_walk_attack_position = Vector2(26, -61)
	
	
	multiple_melee_attack_animations = true
	
	super()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)

func idle_attack_state():
	super()
	if $AnimatedSprite2D.frame == 3:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 2:
		$sfx/range_sfx.play()

func melee_attack_state():
	super()
	if $AnimatedSprite2D.frame == 16:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 15:
		$sfx/melee_sfx.play()

func walk_attack_state(delta):
	super(delta)
	if $AnimatedSprite2D.frame == 17:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 15:
		$sfx/range_sfx.play()
