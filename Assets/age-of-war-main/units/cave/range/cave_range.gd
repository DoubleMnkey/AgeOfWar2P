extends range_unit

# Called when the node enters the scene tree for the first time.
func _ready():
	health = 50
	damage = 5
	move_speed = 50
	money_die_reward = 33
	
	sprite_die_position = Vector2(-20, -40)
	sprite_idle_position = Vector2(0, -43)
	sprite_idle_attack_position = Vector2(27, -43)
	sprite_melee_attack_position = Vector2(17, -52)
	sprite_walk_position = Vector2(0, -44)
	sprite_walk_attack_position = Vector2(21, -43)
	
	super()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)


func idle_attack_state():
	super()
	if $AnimatedSprite2D.frame == 22:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 19:
		$sfx/range_sfx.play()

func melee_attack_state():
	super()
	if $AnimatedSprite2D.frame == 20:
		do_damage($RayCast2D_melee.get_collider())
	elif $AnimatedSprite2D.frame == 17:
		$sfx/melee_sfx.play()

func walk_attack_state(delta):
	super(delta)
	if $AnimatedSprite2D.frame == 23:
		do_damage($RayCast2D_range.get_collider())
	elif $AnimatedSprite2D.frame == 20:
		$sfx/range_sfx.play()
