extends melee_unit


var hit_sfx: AudioStreamPlayer2D

# Called when the node enters the scene tree for the first time.
func _ready():
	health = 600
	damage = 150
	move_speed = 50
	
	sprite_walk_position = Vector2(7, -65)
	sprite_idle_position = Vector2(7, -65)
	sprite_die_position = Vector2(35, -60)
	sprite_attack_position = Vector2(35, -64)
	
	hit_sfx = get_node("sfx/hit_sfx")
	
	money_die_reward = 1300
	
	super()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)


func attack_state():
	if $RayCast2D.is_colliding() == false:
		change_state(state.walk)
		return
	
	if $AnimatedSprite2D.frame == 20:
		hit_sfx.play()
		# whack_sfx.pitch_scale = randf_range(0.9, 1.1)
	if $AnimatedSprite2D.frame == 30:
		do_damage($RayCast2D.get_collider())
