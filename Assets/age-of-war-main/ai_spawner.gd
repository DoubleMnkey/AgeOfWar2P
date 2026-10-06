extends Node2D


var current_age

enum phase {m, r, t}
var current_phase = phase.m
var first_phase_time = 60
var second_phase_time = 60

var enemy_units_container

signal change_age

var unit_timer

# Called when the node enters the scene tree for the first time.
func _ready():
	self.global_position.y = 570
	
	unit_timer = get_node("unit_spawn_timer")
	
	current_age = "cave"
	unit_timer.start(3.0)
	enemy_units_container = get_node("/root/main_game/enemy_units")
	$change_age_timer.wait_time = 180
	$change_age_timer.start()
	$turret_timer.start()
	
	$phase_timer.wait_time = first_phase_time
	$phase_timer.start()



func spawn_melee():
	var cave_melee2 = load("res://units/" + current_age + "/melee/" + current_age + "_melee.tscn").instantiate()
	cave_melee2.position = self.global_position
	cave_melee2.is_player_owned = false
	enemy_units_container.add_child(cave_melee2)
	$EnemySpawnAura.flash()

func spawn_range():
	var cave_range2 = load("res://units/" + current_age + "/range/" + current_age + "_range.tscn").instantiate()
	cave_range2.position = self.global_position
	cave_range2.is_player_owned = false
	enemy_units_container.add_child(cave_range2)
	$EnemySpawnAura.flash()

func spawn_tank():
	var cave_tank2 = load("res://units/" + current_age + "/tank/" + current_age + "_tank.tscn").instantiate()
	cave_tank2.position = self.global_position
	cave_tank2.is_player_owned = false
	enemy_units_container.add_child(cave_tank2)
	$EnemySpawnAura.flash()
	
func buy_turret():
	pass
	
func add_turret_spot():
	pass
	
func sell_turret():
	pass
	
func advance_to_next_age():
	if current_age == "cave":
		current_age = "knight"
	elif current_age == "knight":
		current_age = "medival"
	elif current_age == "medival":
		current_age = "miltary"
	elif current_age == "miltary":
		current_age = "future"


func _on_timer_timeout():
	if current_phase == phase.m:
		spawn_melee()
		unit_timer.wait_time = randf_range(2.0, 8.0)
		
	elif current_phase == phase.r:
		var random_value = randi_range(1, 2)
		if random_value == 1:
			spawn_melee()
			unit_timer.wait_time = randf_range(2.0, 6.0)
		elif random_value == 2:
			spawn_range()
			unit_timer.wait_time = randf_range(3.0, 6.0)
			
	elif current_phase == phase.t:
		var random_value = randi_range(1, 4)
		if random_value == 1:
			spawn_melee()
			unit_timer.wait_time = randf_range(2.0, 4.0)
		elif random_value == 2:
			spawn_range()
			unit_timer.wait_time = randf_range(2.0, 4.0)
		elif random_value == 3 or random_value == 4:
			spawn_tank()
			unit_timer.wait_time = randf_range(4.0, 6.0)

	

# Might remove this
func _on_area_2d_body_entered(body):
	unit_timer.stop()
	

func _on_area_2d_body_exited(body):
	unit_timer.start()

func _on_timer_2_timeout():
	emit_signal("change_age")
	if current_age == "cave":
		current_age = "knight"
		$change_age_timer.wait_time = 200
	elif current_age == "knight":
		current_age = "medival"
		$change_age_timer.wait_time = 220
	elif current_age == "medival":
		current_age = "miltary"
		$change_age_timer.wait_time = 240
	elif current_age == "miltary":
		current_age = "future"
		$change_age_timer.stop()
	reset_phase()


func _on_timer_3_timeout():
	print("do a turret action")
	var enemy_base = get_node("/root/main_game/enemy_base")
	
	var choice = randi_range(1, 4)
	
	# pick from options
	# 1 add turret -> Will only add a turret if space is available
	# 2 upgrade turret -> Will only upgrade turrets if it's current Age is greater than the age of the turret. Upgrading a turret will not go above the age of the turret
	# 3 add a turret spot
	# 4 sell a turret that is of previous age.
	
	if choice == 1: # Buy a turret
		if enemy_base.has_any_empty_tower_spots() == true:
			enemy_base.spawn_ai_turret(current_age) # Pick the first free spot and spawn a turret there
			$turret_timer.wait_time += 15
		else:
			pass
	elif choice == 2: # Upgrade a turret
		enemy_base.upgrade_ai_turret(current_age)
	elif choice == 3:
		var result = enemy_base.add_ai_turret_spot()
		if result == true:
			$turret_timer.wait_time += 15
	elif choice == 4:
		enemy_base.remove_ai_old_turret(current_age)
	



func reset_phase():
	current_phase = phase.m
	$phase_timer.wait_time = first_phase_time
	$phase_timer.start()

func _on_phase_timer_timeout():
	if current_phase == phase.m:
		current_phase = phase.r
		$phase_timer.wait_time = second_phase_time
		$phase_timer.start()
	elif current_phase == phase.r:
		current_phase = phase.t
		$phase_timer.stop()
