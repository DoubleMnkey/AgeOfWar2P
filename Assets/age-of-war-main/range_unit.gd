extends RigidBody2D
class_name range_unit

@export var is_player_owned: bool = true

var max_health
var health
var damage
var move_speed
var starting_health_bar_size

var multiple_melee_attack_animations: bool = false # default to false
enum state {die, idle, idle_attack, melee_attack, walk, walk_attack}
var current_state
var death_timer

# All state positions
var sprite_die_position = Vector2(-20, -40)
var sprite_idle_position = Vector2(0, -43)
var sprite_idle_attack_position = Vector2(27, -43)
var sprite_melee_attack_position = Vector2(17, -52)
var sprite_walk_position = Vector2(0, -44)
var sprite_walk_attack_position = Vector2(21, -43)

var animated_sprite: AnimatedSprite2D
var collision_shape: CollisionShape2D
var range_ray_cast: RayCast2D
var melee_ray_cast: RayCast2D

var die_sfx: AudioStreamPlayer2D

var money_die_reward

# Called when the node enters the scene tree for the first time.
func _ready():
	
	death_timer = get_node("death_timer")
	die_sfx = get_node("sfx/die_sfx")
	range_ray_cast = get_node("RayCast2D_range")
	melee_ray_cast = get_node("RayCast2D_melee")
	animated_sprite = get_node("AnimatedSprite2D")
	collision_shape = get_node("CollisionShape2D")
	
	input_pickable = true
	self.connect("mouse_entered", _on_mouse_entered)
	self.connect("mouse_exited", _on_mouse_exited)
	
	if is_player_owned == false:
		animated_sprite.flip_h = true
		melee_ray_cast.scale.x = -1
		range_ray_cast.scale.x = -1
		z_index = 0
	elif is_player_owned == true:
		animated_sprite.flip_h = false
		melee_ray_cast.scale.x = 1
		range_ray_cast.scale.x = 1
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

	# collide with opposite base
	if is_player_owned == false:
		range_ray_cast.set_collision_mask_value(3, true)
		melee_ray_cast.set_collision_mask_value(3, true)
		self.z_index = 1
	else:
		range_ray_cast.set_collision_mask_value(4, true)
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
	# Check if range_raycast is colliding with friendly unit, if so add it to exceptions
	if range_ray_cast.is_colliding() == true and range_ray_cast.get_collider().is_player_owned == is_player_owned:
		range_ray_cast.add_exception_rid(range_ray_cast.get_collider())
	
	if current_state == state.die:
		pass
	elif current_state == state.idle:
		idle_state()
	elif current_state == state.idle_attack:
		idle_attack_state()
	elif current_state == state.melee_attack:
		melee_attack_state()
	elif current_state == state.walk:
		walk_state(delta)
	elif current_state == state.walk_attack:
		walk_attack_state(delta)
		
	
	if health <= 0 and current_state != state.die:
		change_state(state.die)
		die_sfx.stream = load("res://age of war sprites/audio/sfx/die_0" + str(randi_range(1,5)) + ".mp3")
		die_sfx.play()
		
		if is_player_owned == false:
			GlobalVariables.player_money += money_die_reward
			GlobalVariables.player_exp += 2 * money_die_reward
			spawn_show_death_money()
		else:
			GlobalVariables.player_exp += int (money_die_reward/2)
	
	position.y = 570
	$Label.text = str(health)


# handle the states

func idle_state():
	if $RayCast2D_melee.is_colliding() == false:
		change_state(state.walk)
	elif $RayCast2D_melee.is_colliding() == true and $RayCast2D_melee.get_collider().is_player_owned == is_player_owned and $RayCast2D_range.is_colliding():
		change_state(state.idle_attack)

func idle_attack_state():
	if $RayCast2D_range.is_colliding() == false:
		change_state(state.walk)
	
	if $RayCast2D_melee.is_colliding() == false:
		change_state(state.walk_attack)
		
	

func melee_attack_state():
	if $RayCast2D_melee.is_colliding() == false and $RayCast2D_range.is_colliding() == true:
		change_state(state.walk_attack)
	elif $RayCast2D_melee.is_colliding() == false and $RayCast2D_range.is_colliding() == false:
		change_state(state.walk)
	
	

func walk_state(delta):
	move(delta)
		
	# First check if it is on the same team, if yes - add it to the exception
	if $RayCast2D_range.is_colliding() == true and $RayCast2D_range.get_collider().is_player_owned == is_player_owned:
		$RayCast2D_range.add_exception_rid($RayCast2D_range.get_collider())
	elif $RayCast2D_range.is_colliding() == true and $RayCast2D_range.get_collider().is_player_owned != is_player_owned:
		if $RayCast2D_melee.is_colliding() == true and $RayCast2D_melee.get_collider().is_player_owned == self.is_player_owned and ($RayCast2D_melee.get_collider().is_walking_or_walk_attacking()):
			change_state(state.walk_attack)
		elif $RayCast2D_melee.is_colliding() == false:
			change_state(state.walk_attack)
			
		
		
	if $RayCast2D_melee.is_colliding() == true:
		if $RayCast2D_melee.get_collider().is_player_owned != is_player_owned and $RayCast2D_melee.get_collider().current_state != $RayCast2D_melee.get_collider().state.die:
			change_state(state.melee_attack)
		elif $RayCast2D_melee.get_collider().is_player_owned == is_player_owned and ($RayCast2D_melee.get_collider().is_idle_or_idle_attacking()):
			change_state(state.idle)

func walk_attack_state(delta):
	move(delta)
		
	if $RayCast2D_range.is_colliding() == false:
		change_state(state.walk)

	if $RayCast2D_melee.is_colliding() == true:
		if $RayCast2D_melee.get_collider().is_player_owned != is_player_owned and $RayCast2D_melee.get_collider().current_state != $RayCast2D_melee.get_collider().state.die:
			change_state(state.melee_attack)
			
		if $RayCast2D_melee.get_collider().is_player_owned == is_player_owned and $RayCast2D_melee.get_collider().is_idle_or_idle_attacking():
			change_state(state.idle_attack)
	
	









func move(delta):
	if is_player_owned == true:
		move_and_collide(Vector2.RIGHT * move_speed * delta)
	elif is_player_owned == false:
		move_and_collide(Vector2.LEFT * move_speed * delta)


func _on_animated_sprite_2d_animation_finished():
	if current_state == state.die:
		death_timer.start()


func _on_death_timer_timeout():
	self.queue_free()

func spawn_show_death_money():
	var effect = load("res://show_death_money.tscn").instantiate()
	effect.global_position = $Control.global_position
	effect.get_node("Label").text = " +" + str(money_die_reward)
	get_parent().add_child(effect)

func is_idle_or_idle_attacking():
	if current_state == state.idle or current_state == state.idle_attack or current_state == state.melee_attack:
		return true
	else:
		return false

func is_walking_or_walk_attacking():
	if current_state == state.walk or current_state == state.walk_attack:
		return true
	else:
		return false


func stop_all_sfx():
	for child in get_node("sfx").get_children():
		child.stop()

func take_damage(outside_damage):
	health -= outside_damage
	$Control/health_bar.size.x = 48 * health / max_health

func do_damage(unit_to_be_damaged):
	unit_to_be_damaged.take_damage(damage)


func _on_animated_sprite_2d_animation_looped():
	if current_state == state.melee_attack and (animated_sprite.get_animation() == "melee_attack_1" or animated_sprite.get_animation() == "melee_attack_2"):
		# randomly pick between the two animations and play the attack animation again
		animated_sprite.play("melee_attack_" + str(randi_range(1, 2)))



## Change states
func change_state(new_state):
	if new_state == state.die:
		change_to_die_state()
	elif new_state == state.idle:
		change_to_idle_state()
	elif new_state == state.idle_attack:
		change_to_idle_attack_state()
	elif new_state == state.melee_attack:
		change_to_melee_attack_state()
	elif new_state == state.walk:
		change_to_walk_state()
	elif new_state == state.walk_attack:
		change_to_walk_attack_state()


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

func change_to_idle_attack_state():
	current_state = state.idle_attack
	animated_sprite.position = sprite_idle_attack_position
	if animated_sprite.flip_h == true:
		animated_sprite.position.x = -animated_sprite.position.x
	animated_sprite.play("idle_attack")

func change_to_melee_attack_state():
	current_state = state.melee_attack
	animated_sprite.position = sprite_melee_attack_position
	if animated_sprite.flip_h == true:
		animated_sprite.position.x = -animated_sprite.position.x
	if multiple_melee_attack_animations == true:
		animated_sprite.play("melee_attack_1")
	else:
		animated_sprite.play("melee_attack")

func change_to_walk_state():
	current_state = state.walk
	animated_sprite.position = sprite_walk_position
	if animated_sprite.flip_h == true:
		animated_sprite.position.x = -animated_sprite.position.x
	animated_sprite.play("walk")
	
func change_to_walk_attack_state():
	current_state = state.walk_attack
	animated_sprite.position = sprite_walk_attack_position
	if animated_sprite.flip_h == true:
		animated_sprite.position.x = -animated_sprite.position.x
	animated_sprite.play("walk_attack")

func _on_mouse_entered():
	$Control.show()
	$Control/health_bar.size.x = starting_health_bar_size * health / max_health

func _on_mouse_exited():
	$Control.hide()
