extends melee_unit

var hit_sfx

# Called when the node enters the scene tree for the first time.
func _ready():
	health = 120
	damage = 40
	move_speed = 50
	money_die_reward = 130
	
	sprite_walk_position = Vector2(0, -65)
	sprite_idle_position = Vector2(-1, -65)
	sprite_die_position = Vector2(-48, -65)
	sprite_attack_position = Vector2(17, -65)
	
	hit_sfx = get_node("sfx/hit_sfx")
	
	super()
	custom_death_sfx = "res://age of war sprites/audio/sfx/cave_tank_die.mp3"


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	super(delta)


func attack_state():
	if $RayCast2D.is_colliding() == false:
		change_state(state.walk)
		stop_all_sfx()
		return
	
	if $AnimatedSprite2D.frame == 18:
		hit_sfx.play()
		# whack_sfx.pitch_scale = randf_range(0.9, 1.1)
	if $AnimatedSprite2D.frame == 21:
		do_damage($RayCast2D.get_collider())
