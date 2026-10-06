extends RigidBody2D
class_name melee_unit

@export var is_player_owned: bool = true

var multiple_attack_animations: bool = false # default to false
var custom_death_sfx = null
var damage_frame

var max_health
var health
var damage
var move_speed
var starting_health_bar_size

enum state {attack, die, idle, walk}
var current_state
var death_timer
var die_sfx

var sprite_walk_position : Vector2
var sprite_idle_position : Vector2
var sprite_die_position : Vector2
var sprite_attack_position : Vector2
var animated_sprite : AnimatedSprite2D
var melee_ray_cast : RayCast2D

var collision_shape : CollisionShape2D


## die variables
var money_die_reward

var damage_1_frame = null
var hit_sfx_1_frame = null
var whoosh_sfx_1_frame = null

# Called when the node enters the scene tree for the first time.
func _ready():
	# Get nodes
	death_timer = get_node("death_timer")
	die_sfx = get_node("sfx/die_sfx")
	melee_ray_cast = get_node("RayCast2D")
	animated_sprite = get_node("AnimatedSprite2D")
	collision_shape = get_node("CollisionShape2D")
	
	
	input_pickable = true
	self.connect("mouse_entered", _on_mouse_entered)
	self.connect("mouse_exited", _on_mouse_exited)
	
	
	if is_player_owned == false:
		animated_sprite.flip_h = true
		melee_ray_cast.scale.x = -1
		z_index = 0
	elif is_player_owned == true:
		animated_sprite.flip_h = false
		melee_ray_cast.scale.x = 1
		z_index = 1
	change_to_walk_state()
	
	if not death_timer.is_connected("timeout", _on_death_timer_timeout):
		death_timer.connect("timeout", _on_death_timer_timeout)
	
	if not animated_sprite.is_connected("animation_finished", _on_animated_sprite_2d_animation_finished):
		animated_sprite.connect("animation_finished", _on_animated_sprite_2d_animation_finished)
	
	if not animated_sprite.is_connected("animation_looped", _on_animated_sprite_2d_animation_looped):
		animated_sprite.connect("animation_looped", _on_animated_sprite_2d_animation_looped)
	
	for child in get_node("sfx").get_children():
		child.bus = &'sfx'
		
	# lock rotation
	lock_rotation = true
	
	gravity_scale = 0
	if is_player_owned == false:
		melee_ray_cast.set_collision_mask_value(3, true)
		self.z_index = 1
	else:
		melee_ray_cast.set_collision_mask_value(4, true)
		self.z_index = 2
	if is_player_owned == false and GlobalVariables.current_difficulty == GlobalVariables.difficulty.hard:
		health *= 1.25
	elif is_player_owned == false and GlobalVariables.current_difficulty == GlobalVariables.difficulty.impossible:
		health *= 1.5
		
	max_health = health
	starting_health_bar_size = $Control/health_bar.size.x
	$Control.hide()
	$Label.hide()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if current_state == state.walk:
		move_state(delta)
	elif current_state == state.attack:
		attack_state()
	elif current_state == state.die:
		pass
	elif current_state == state.idle:
		idle_state()
			
	if health <= 0 and current_state != state.die:
		change_state(state.die)
		if custom_death_sfx == null:
			die_sfx.stream = load("res://age of war sprites/audio/sfx/die_0" + str(randi_range(1,5)) + ".mp3")
		else:
			die_sfx.stream = load(custom_death_sfx)
		# stop all sfx
		stop_all_sfx()
		die_sfx.play()
	
		
		if is_player_owned == false:
			GlobalVariables.player_money += money_die_reward
			GlobalVariables.player_exp += 2 * money_die_reward
			spawn_show_death_money()
		else:
			GlobalVariables.player_exp += int (money_die_reward/2)

	position.y = 570
	$Label.text = str(health)
	
func spawn_show_death_money():
	var effect = load("res://show_death_money.tscn").instantiate()
	effect.global_position = $Control.global_position
	effect.get_node("Label").text = " +" + str(money_die_reward)
	get_parent().add_child(effect)

func attack_state():
	pass

func take_damage(outside_damage):
	health -= outside_damage
	$Control/health_bar.size.x = 48 * health / max_health
	
func do_damage(unit_to_be_damaged):
	unit_to_be_damaged.take_damage(damage)

func is_idle_or_idle_attacking():
	if current_state == state.idle or current_state == state.attack:
		return true
	else:
		return false

func is_walking_or_walk_attacking():
	if current_state == state.walk:
		return true
	else:
		return false

func move_state(delta):
	if $RayCast2D.is_colliding() == true:
		if $RayCast2D.get_collider().is_player_owned != is_player_owned and $RayCast2D.get_collider().current_state != $RayCast2D.get_collider().state.die:
			change_state(state.attack)
		elif $RayCast2D.get_collider().is_player_owned == is_player_owned and ($RayCast2D.get_collider().is_idle_or_idle_attacking()):
			change_state(state.idle)
	else:
		if is_player_owned == true:
			move_and_collide(Vector2.RIGHT * move_speed * delta)
		elif is_player_owned == false:
			move_and_collide(Vector2.LEFT * move_speed * delta)

func idle_state():
	# There is a friendly unit infront of us, There is not too much we can do until the unit infront of us moves
	if $RayCast2D.is_colliding() == false:
		change_state(state.walk)

func change_state(new_state):
	if new_state == state.attack:
		change_to_attack_state()
	elif new_state == state.die:
		change_to_die_state()
	elif new_state == state.idle:
		change_to_idle_state()
	elif new_state == state.walk:
		change_to_walk_state()

func change_to_attack_state():
	current_state = state.attack
	animated_sprite.position = sprite_attack_position
	if animated_sprite.flip_h == true:
		animated_sprite.position.x = -animated_sprite.position.x
	if multiple_attack_animations == true:
		animated_sprite.play("attack_1")
	else:
		animated_sprite.play("attack")

func change_to_die_state():
	current_state = state.die
	animated_sprite.position = sprite_die_position
	if animated_sprite.flip_h == true:
		animated_sprite.position.x = -animated_sprite.position.x
	animated_sprite.play("die")
	collision_shape.disabled = true
	animated_sprite.z_index = -10

func change_to_idle_state():
	current_state = state.idle
	animated_sprite.position = sprite_idle_position
	if animated_sprite.flip_h == true:
		animated_sprite.position.x = -animated_sprite.position.x
	animated_sprite.play("idle")

func change_to_walk_state():
	current_state = state.walk
	animated_sprite.position = sprite_walk_position
	if animated_sprite.flip_h == true:
		animated_sprite.position.x = -animated_sprite.position.x
	animated_sprite.play("walk")

func _on_death_timer_timeout():
	self.queue_free()

func _on_animated_sprite_2d_animation_finished():
	if current_state == state.die:
		death_timer.start()
		
		
func _on_animated_sprite_2d_animation_looped():
	if current_state == state.attack and (animated_sprite.get_animation() == "attack_1" or animated_sprite.get_animation() == "attack_2"):
		# randomly pick between the two animations and play the attack animation again
		animated_sprite.play("attack_" + str(randi_range(1, 2)))

func stop_all_sfx():
	for child in get_node("sfx").get_children():
		child.stop()


func _on_mouse_entered():
	$Control.show()
	$Control/health_bar.size.x = starting_health_bar_size * health / max_health

func _on_mouse_exited():
	$Control.hide()

# returns the left most position of the collision box
# Used for units that are overlapping
func get_left_most_position():
	pass
