extends StaticBody2D

@export var is_player_owned: bool = true

var health
var max_health
var turret_array : Array
var turret_data : Array

# I am so stupid -> this is required so units can attack the base
var current_state = 9999
enum state {attack, die, idle, walk}

# Called when the node enters the scene tree for the first time.
func _ready():
	health = 500
	max_health = 500
	# -1 unavailable to build (because no space)
	# 0 means no turret (but there is space to build)
	# 1 means there is a turret
	# first element is for base
	# second is for bottom
	# third is for part
	# fourth is for top
	turret_array = [0,-1,-1,-1]
	turret_data = [null, null, null, null]
	$tower_bottom.hide()
	$tower_part.hide()
	$tower_top.hide()
	
	if scale.x == -1:
		$Label.scale.x = -1
		$Label.position.x = -20
	$Label.text = str(health)
	
	deactivate_buttons()


func add_turret_spot():
	if turret_array[1] == -1:
		$tower_bottom.show()
		turret_array[1] = 0
	elif turret_array[2] == -1:
		$tower_part.show()
		turret_array[2] = 0
	elif turret_array[3] == -1:
		$tower_top.show()
		turret_array[3] = 0


func _on_button_tower_base_pressed():
	if turret_array[0] == 0: # means we are buying the turret
		var turret_name = get_node("/root/main_game/sprite_follow_player_mouse").turret_name
		get_node("/root/main_game/sprite_follow_player_mouse").queue_free()
		get_node("/root/main_game/Camera2D/in_game_menu").hide_turret_cancel_button_and_show_turret_menu()
		deactivate_buttons()
		# print(turret_name)
		var turret_base = load("res://bases/" + turret_name + ".tscn").instantiate()
		turret_base.name += "_" + str(randi())
		self.add_child(turret_base)
		turret_base.position = get_node("button_container").get_child(0).position + Vector2(16, 16)
		turret_array[0] = 1
		turret_data[0] = turret_base
	elif turret_array[0] == 1: # means we are selling the turret
		deactivate_buttons()
		var stage = turret_data[0].name.split("_")[0]
		var turret_number = int(turret_data[0].name.split("_")[2])
		var refund = GlobalVariables.get_turret_price(GlobalVariables.get_stage_from_string(stage), turret_number) / 2
		GlobalVariables.player_money += refund
		turret_data[0].queue_free()
		turret_data[0] = null
		turret_array[0] = 0
		get_node("/root/main_game/Camera2D/in_game_menu")._on_cancel_sell_turret_pressed()


func activate_turret_buy_buttons():
	if turret_array[0] == 0:
		get_node("button_container").get_child(0).disabled = false
	
	if turret_array[1] == 0:
		get_node("button_container").get_child(1).disabled = false
	
	if turret_array[2] == 0:
		get_node("button_container").get_child(2).disabled = false
	
	if turret_array[3] == 0:
		get_node("button_container").get_child(3).disabled = false

func activate_turret_sell_buttons():
	# same buttons lol
	if turret_array[0] == 1:
		get_node("button_container").get_child(0).disabled = false
	
	if turret_array[1] == 1:
		get_node("button_container").get_child(1).disabled = false
	
	if turret_array[2] == 1:
		get_node("button_container").get_child(2).disabled = false
	
	if turret_array[3] == 1:
		get_node("button_container").get_child(3).disabled = false

func activate_buttons():
	for button in get_node("button_container").get_children():
		button.disabled = false

func deactivate_buttons():
	for button in get_node("button_container").get_children():
		button.disabled = true

func hide_buttons():
	for button in get_node("button_container").get_children():
		button.hide()


func _on_button_tower_bottom_pressed():
	if turret_array[1] == 0: # means we are buying the turret
		var turret_name = get_node("/root/main_game/sprite_follow_player_mouse").turret_name
		get_node("/root/main_game/sprite_follow_player_mouse").queue_free()
		get_node("/root/main_game/Camera2D/in_game_menu").hide_turret_cancel_button_and_show_turret_menu()
		deactivate_buttons()
		var turret_base = load("res://bases/" + turret_name + ".tscn").instantiate()
		turret_base.name += "_" + str(randi())
		self.add_child(turret_base)
		turret_base.position = get_node("button_container").get_child(1).position + Vector2(16, 16)
		turret_array[1] = 1
		turret_data[1] = turret_base
	elif turret_array[1] == 1: # means we are selling the turret
		deactivate_buttons()
		var stage = turret_data[1].name.split("_")[0]
		var turret_number = int(turret_data[1].name.split("_")[2])
		var refund = GlobalVariables.get_turret_price(GlobalVariables.get_stage_from_string(stage), turret_number) / 2
		GlobalVariables.player_money += refund
		turret_data[1].queue_free()
		turret_data[1] = null
		turret_array[1] = 0
		get_node("/root/main_game/Camera2D/in_game_menu")._on_cancel_sell_turret_pressed()

func take_damage(damage):
	health -= damage
	$PanelContainer/current_health.custom_minimum_size.y = 250 * health/max_health
	$Label.text = str(health)
	if health <= 0:
		if is_player_owned == true:
			MusicManager.audioStreamPlayer.stop()
			get_tree().change_scene_to_file("res://scenes/game_over.tscn")
		elif is_player_owned == false:
			MusicManager.audioStreamPlayer.stop()
			get_tree().change_scene_to_file("res://scenes/win_screen.tscn")

func get_number_of_turret_slots():
	var number = 0
	for i in turret_array:
		if i == 0 or i == 1:
			number += 1
	return number

func get_price_of_new_turret_slot():
	var number = get_number_of_turret_slots()
	if number == 1:
		return 1000
	elif number == 2:
		return 3000
	elif number == 3:
		return 7500
	elif number == 4:
		return INF


func _on_button_tower_part_pressed():
	if turret_array[2] == 0: # means we are buying the turret
		var turret_name = get_node("/root/main_game/sprite_follow_player_mouse").turret_name
		get_node("/root/main_game/sprite_follow_player_mouse").queue_free()
		get_node("/root/main_game/Camera2D/in_game_menu").hide_turret_cancel_button_and_show_turret_menu()
		deactivate_buttons()
		var turret_base = load("res://bases/" + turret_name + ".tscn").instantiate()
		turret_base.name += "_" + str(randi())
		self.add_child(turret_base)
		turret_base.position = get_node("button_container").get_child(2).position + Vector2(16, 16)
		turret_array[2] = 1
		turret_data[2] = turret_base
	elif turret_array[2] == 1: # means we are selling the turret
		deactivate_buttons()
		var stage = turret_data[2].name.split("_")[0]
		var turret_number = int(turret_data[2].name.split("_")[2])
		var refund = GlobalVariables.get_turret_price(GlobalVariables.get_stage_from_string(stage), turret_number) / 2
		GlobalVariables.player_money += refund
		turret_data[2].queue_free()
		turret_data[2] = null
		turret_array[2] = 0
		get_node("/root/main_game/Camera2D/in_game_menu")._on_cancel_sell_turret_pressed()


func _on_button_tower_top_pressed():
	if turret_array[3] == 0: # means we are buying the turret
		var turret_name = get_node("/root/main_game/sprite_follow_player_mouse").turret_name
		get_node("/root/main_game/sprite_follow_player_mouse").queue_free()
		get_node("/root/main_game/Camera2D/in_game_menu").hide_turret_cancel_button_and_show_turret_menu()
		deactivate_buttons()
		var turret_base = load("res://bases/" + turret_name + ".tscn").instantiate()
		turret_base.name += "_" + str(randi())
		self.add_child(turret_base)
		turret_base.position = get_node("button_container").get_child(3).position + Vector2(16, 16)
		turret_array[3] = 1
		turret_data[3] = turret_base
	elif turret_array[3] == 1: # means we are selling the turret
		deactivate_buttons()
		var stage = turret_data[3].name.split("_")[0]
		var turret_number = int(turret_data[3].name.split("_")[2])
		var refund = GlobalVariables.get_turret_price(GlobalVariables.get_stage_from_string(stage), turret_number) / 2
		GlobalVariables.player_money += refund
		turret_data[3].queue_free()
		turret_data[3] = null
		turret_array[3] = 0
		get_node("/root/main_game/Camera2D/in_game_menu")._on_cancel_sell_turret_pressed()

func advance_base_sprite():
	if GlobalVariables.current_stage == GlobalVariables.stage.knight:
		$base_main_sprite.texture = load("res://age of war sprites/bases/knight/base/base.png")
		$tower_bottom.texture = load("res://age of war sprites/bases/knight/tower_base/base_tower_bottom.png")
		$tower_part.texture = load("res://age of war sprites/bases/knight/tower_part/base_tower_part.png")
		$tower_top.texture = load("res://age of war sprites/bases/knight/tower_top/base_tower_top.png")
		health += 600
		max_health = 1100
		$PanelContainer/current_health.custom_minimum_size.y = 250 * health/max_health
	elif GlobalVariables.current_stage == GlobalVariables.stage.medival:
		$base_main_sprite.texture = load("res://age of war sprites/bases/medival/base/base.png")
		$tower_bottom.texture = load("res://age of war sprites/bases/medival/tower_base/base_tower_bottom.png")
		$tower_part.texture = load("res://age of war sprites/bases/medival/tower_part/base_tower_part.png")
		$tower_top.texture = load("res://age of war sprites/bases/medival/tower_top/base_tower_top.png")
		health += 900
		max_health = 2000
		$PanelContainer/current_health.custom_minimum_size.y = 250 * health/max_health
	elif GlobalVariables.current_stage == GlobalVariables.stage.miltary:
		$base_main_sprite.texture = load("res://age of war sprites/bases/miltary/base/base.png")
		$tower_bottom.texture = load("res://age of war sprites/bases/miltary/tower_base/base_tower_bottom.png")
		$tower_part.texture = load("res://age of war sprites/bases/miltary/tower_part/base_tower_part.png")
		$tower_top.texture = load("res://age of war sprites/bases/miltary/tower_top/base_tower_top.png")
		health += 1200
		max_health = 3200
		$PanelContainer/current_health.custom_minimum_size.y = 250 * health/max_health
	elif GlobalVariables.current_stage == GlobalVariables.stage.future:
		$base_main_sprite.texture = load("res://age of war sprites/bases/future/base/base.png")
		$tower_bottom.texture = load("res://age of war sprites/bases/future/tower_base/base_tower_bottom.png")
		$tower_part.texture = load("res://age of war sprites/bases/future/tower_part/base_tower_part.png")
		$tower_top.texture = load("res://age of war sprites/bases/future/tower_top/base_tower_top.png")
		health += 1500
		max_health = 4700
		$PanelContainer/current_health.custom_minimum_size.y = 250 * health/max_health
	$Label.text = str(health)

func update_sprite_ai():
	if max_health == 500:
		$base_main_sprite.texture = load("res://age of war sprites/bases/knight/base/base.png")
		$tower_bottom.texture = load("res://age of war sprites/bases/knight/tower_base/base_tower_bottom.png")
		$tower_part.texture = load("res://age of war sprites/bases/knight/tower_part/base_tower_part.png")
		$tower_top.texture = load("res://age of war sprites/bases/knight/tower_top/base_tower_top.png")
		health += 600
		max_health = 1100
		$PanelContainer/current_health.custom_minimum_size.y = 250 * health/max_health
	elif max_health == 1100:
		$base_main_sprite.texture = load("res://age of war sprites/bases/medival/base/base.png")
		$tower_bottom.texture = load("res://age of war sprites/bases/medival/tower_base/base_tower_bottom.png")
		$tower_part.texture = load("res://age of war sprites/bases/medival/tower_part/base_tower_part.png")
		$tower_top.texture = load("res://age of war sprites/bases/medival/tower_top/base_tower_top.png")
		health += 900
		max_health = 2000
		$PanelContainer/current_health.custom_minimum_size.y = 250 * health/max_health
	elif max_health == 2000:
		$base_main_sprite.texture = load("res://age of war sprites/bases/miltary/base/base.png")
		$tower_bottom.texture = load("res://age of war sprites/bases/miltary/tower_base/base_tower_bottom.png")
		$tower_part.texture = load("res://age of war sprites/bases/miltary/tower_part/base_tower_part.png")
		$tower_top.texture = load("res://age of war sprites/bases/miltary/tower_top/base_tower_top.png")
		health += 1200
		max_health = 3200
		$PanelContainer/current_health.custom_minimum_size.y = 250 * health/max_health	
	elif max_health == 3200:
		$base_main_sprite.texture = load("res://age of war sprites/bases/future/base/base.png")
		$tower_bottom.texture = load("res://age of war sprites/bases/future/tower_base/base_tower_bottom.png")
		$tower_part.texture = load("res://age of war sprites/bases/future/tower_part/base_tower_part.png")
		$tower_top.texture = load("res://age of war sprites/bases/future/tower_top/base_tower_top.png")
		health += 1500
		max_health = 4700
		$PanelContainer/current_health.custom_minimum_size.y = 250 * health/max_health
	$Label.text = str(health)

### AI ###
# Functions used for the enemy base

func spawn_ai_turret(age: String):
	var index = get_next_available_spot()
	if index == null:
		return
	else:
		var turret_base = load("res://bases/" + age + "_turret_1" + ".tscn").instantiate()
		turret_base.name += "_" + str(randi()) # We have to do this to avoid multiple same turrets with colliding names
		self.add_child(turret_base)
		turret_base.is_player_owned = false
		turret_base.position = get_node("button_container").get_child(index).position + Vector2(16, 16)
		turret_array[index] = 1
		turret_data[index] = turret_base
	

func upgrade_ai_turret(age):
	var turret_index = pick_random_turret()
	if turret_index == null:
		return
	
	var turret_age = get_turret_age(turret_index)
	if turret_age == null:
		return
	if turret_age != age:
		print("upgrade turret -> bring turret to the new age")
		turret_data[turret_index].queue_free()
		turret_data[turret_index] = null
		turret_array[turret_index] = 0
		
		var turret_base = load("res://bases/" + age + "_turret_1" + ".tscn").instantiate()
		turret_base.name += "_" + str(randi()) # We have to do this to avoid multiple same turrets with colliding names
		self.add_child(turret_base)
		turret_base.is_player_owned = false
		turret_base.position = get_node("button_container").get_child(turret_index).position + Vector2(16, 16)
		turret_array[turret_index] = 1
		turret_data[turret_index] = turret_base
	else:
		print("upgrade turret tier")
		var current_tier = int(get_turret_tier(turret_index))
		if current_tier == 3:
			return
		
		turret_data[turret_index].queue_free()
		turret_data[turret_index] = null
		turret_array[turret_index] = 0
		
		var tier = str(int(current_tier) + 1)
		
		var turret_base = load("res://bases/" + age + "_turret_" + tier + ".tscn").instantiate()
		turret_base.name += "_" + str(randi()) # We have to do this to avoid multiple same turrets with colliding names
		self.add_child(turret_base)
		turret_base.is_player_owned = false
		turret_base.position = get_node("button_container").get_child(turret_index).position + Vector2(16, 16)
		turret_array[turret_index] = 1
		turret_data[turret_index] = turret_base

# Pick a random turret to upgrade
func pick_random_turret():
	var i = randi_range(0, 3)
	if turret_array[i] == 1:
		return i
	else:
		# There is no turret to upgrade here, skip
		return null 

# returns the age of the indexed turret as a string
func get_turret_age(index: int):
	if turret_array[index] != 1:
		return null
	var turret_obj = turret_data[index]
	return turret_obj.name.split("_")[0]

func get_turret_tier(index: int):
	if turret_array[index] != 1:
		return
	var turret_obj = turret_data[index]
	return turret_obj.name.split("_")[2]

# Used for placing a new turret
func has_any_empty_tower_spots():
	for i in turret_array:
		if i == 0:
			return true
	return false

func add_ai_turret_spot():
	if turret_array[1] == -1:
		$tower_bottom.show()
		turret_array[1] = 0
		return true
	elif turret_array[2] == -1:
		$tower_part.show()
		turret_array[2] = 0
		return true
	elif turret_array[3] == -1:
		$tower_top.show()
		turret_array[3] = 0
		return true
	return false
	
func remove_ai_old_turret(input_age):
	print("remove old turret if any exist")
	for i in range(0,4):
		
		var age = get_turret_age(i)
		if age == null:
			continue
		if age != input_age:
			turret_data[i].queue_free()
			turret_data[i] = null
			turret_array[i] = 0
			return


# returns the index of the next available spot
func get_next_available_spot():
	var index = 0
	while index < turret_array.size():
		if turret_array[index] == 0:
			return index
		index += 1 
	return null
