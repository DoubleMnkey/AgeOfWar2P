extends melee_unit

var hit_sfx


# Called when the node enters the scene tree for the first time.
func _ready():
	health = 70
	damage = 10
	move_speed = 50
	money_die_reward = 20
	
	sprite_walk_position = Vector2(0, -44)
	sprite_idle_position =  Vector2(0, -44)
	sprite_die_position = Vector2(-20, -44)
	sprite_attack_position = Vector2(8, -56)
	
	hit_sfx = get_node("sfx/hit_sfx")
	
	
	super()
	if is_player_owned == false:
		$Shadow.position.x = -$Shadow.position.x
	
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)

func attack_state():
	if $RayCast2D.is_colliding() == false:
		change_state(state.walk)
		stop_all_sfx()
		return
	
	if $AnimatedSprite2D.frame == 12:
		hit_sfx.play()
		# whack_sfx.pitch_scale = randf_range(0.9, 1.1)
	if $AnimatedSprite2D.frame == 19:
		do_damage($RayCast2D.get_collider())


	
