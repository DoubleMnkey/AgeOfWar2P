extends melee_unit


var hit_sfx : AudioStreamPlayer2D

# Called when the node enters the scene tree for the first time.
func _ready():
	health = 1500
	damage = 300
	move_speed = 50
	
	sprite_walk_position = Vector2(34, -53)
	sprite_idle_position = Vector2(34, -53)
	sprite_die_position = Vector2(31, -144)
	sprite_attack_position = Vector2(74, -61)
	
	hit_sfx = get_node("sfx/hit_sfx")
	
	money_die_reward = 9100
	
	super()
	custom_death_sfx = "res://age of war sprites/audio/sfx/explosion_02.mp3"

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
	if $AnimatedSprite2D.frame == 24:
		do_damage($RayCast2D.get_collider())

func _on_animated_sprite_2d_animation_finished():
	self.queue_free()
