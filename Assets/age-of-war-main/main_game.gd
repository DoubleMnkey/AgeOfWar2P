extends Node2D

@onready var player_unit_spawn_position : Vector2 = $player_spawn_location.global_position
var unit_array : Array
var player_spawn_location_occupied : bool = false
var unable_to_spawn
var medival_special_active : bool

# Called when the node enters the scene tree for the first time.
func _ready():
	$Camera2D.global_position = Vector2(576, 280)
	$Camera2D/in_game_menu.connect("spawn_melee", _on_melee_button_pressed)
	$Camera2D/in_game_menu.connect("spawn_range", _on_range_button_pressed)
	$Camera2D/in_game_menu.connect("spawn_tank", _on_tank_button_pressed)
	$Camera2D/in_game_menu.connect("spawn_super_soldier", _on_super_soldier_button_pressed)
	unable_to_spawn = false
	medival_special_active = false
	$ai_spawner.connect("change_age", _on_ai_change_age)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	$Camera2D/in_game_menu/money.text = str(GlobalVariables.player_money)
	$Camera2D/in_game_menu/exp.text = str(GlobalVariables.player_exp)
	if medival_special_active == true:
		if $medival_special_timer.is_stopped() == true:
			$medival_special_timer.start(5.0)
		for unit in get_node("player_units").get_children():
			# Might want to remove the if statement if I want overhealing to be in the game.
			# Overhealling is actually really good, all specials should be a "game saver".
			unit.max_health += 1
			unit.take_damage(-1)
			if unit.get_node("heal_sprite") == null:
				var sprite = Sprite2D.new()
				sprite.texture = load("res://age of war sprites/effects/heal/medival_special_heal.png")
				sprite.offset = Vector2(0, -74)
				sprite.scale = Vector2(0.8, 0.8)
				sprite.name = "heal_sprite"
				unit.add_child(sprite)


func get_first_player_unit():
	pass
	
func get_first_enemy_unit():
	pass

func get_last_player_unit():
	pass
	
func get_last_enemy_unit():
	pass

func _on_melee_button_pressed(stage: String):
	if unable_to_spawn == true:
		return
	var new_melee_unit = load("res://units/" + stage + "/melee/" + stage + "_melee.tscn").instantiate()
	new_melee_unit.position = player_unit_spawn_position
	new_melee_unit.position.x -= 32
	new_melee_unit.is_player_owned = true
	get_node("/root/main_game/player_units").add_child(new_melee_unit)
	%PlayerSpawnAura.flash()

func _on_range_button_pressed(stage: String):
	if unable_to_spawn == true:
		return
	var cave_range = load("res://units/" + stage + "/range/" + stage + "_range.tscn").instantiate()
	cave_range.position = player_unit_spawn_position
	cave_range.position.x -= 32
	cave_range.is_player_owned = true
	get_node("/root/main_game/player_units").add_child(cave_range)
	%PlayerSpawnAura.flash()

func _on_tank_button_pressed(stage: String):
	if unable_to_spawn == true:
		return
	var cave_tank = load("res://units/" + stage + "/tank/" + stage + "_tank.tscn").instantiate()
	cave_tank.position = player_unit_spawn_position
	cave_tank.position.x -= 32
	cave_tank.is_player_owned = true
	get_node("/root/main_game/player_units").add_child(cave_tank)
	%PlayerSpawnAura.flash()
	
# Unused variable stage in this function
func _on_super_soldier_button_pressed(stage: String):
	if unable_to_spawn == true:
		return
	var super_soldier = load("res://units/future/super_soldier/future_super_soldier.tscn").instantiate()
	super_soldier.position = player_unit_spawn_position
	super_soldier.position.x -= 32
	super_soldier.is_player_owned = true
	get_node("/root/main_game/player_units").add_child(super_soldier)
	%PlayerSpawnAura.flash()


func _on_player_spawn_location_body_entered(body):
	if body.is_player_owned == true:
		unable_to_spawn = true


func _on_player_spawn_location_body_exited(body):
	if body.is_player_owned == true:
		unable_to_spawn = false
	
func spawn_random_projectiles_from_sky():
	var i = 0
	while i < 25:
		var projectile = load("res://projectile.tscn").instantiate()
		projectile.global_position = Vector2(randf_range(300, 1500), randf_range(-200, -1000))
		projectile.direction = Vector2(randf_range(-0.5, 0.5), randf_range(2, 4)).normalized()
		projectile.damage = 60
		projectile.speed = 300
		projectile.is_player_owned = true
		projectile.spawn_offspring = false
		projectile.time_to_die = 8.0
		projectile.get_node("Sprite2D").texture = load("res://age of war sprites/bases/medival/turret_3/medival_turret_3_projectile_offspring.png")
		get_node("/root/main_game").add_child(projectile)
		i += 1

func cave_special_attack():
	var i = 0
	while i < 25:
		var projectile = load("res://cave_special_projectile.tscn").instantiate()
		projectile.global_position = Vector2(randf_range(300, 1500), randf_range(-200, -1000))
		projectile.direction = Vector2(randf_range(-0.5, 0.5), randf_range(2, 4)).normalized()
		projectile.damage = 80
		projectile.speed = randi_range(250, 350)
		projectile.is_player_owned = true
		projectile.spawn_offspring = false
		projectile.time_to_die = 8.0
		projectile.get_node("Sprite2D").rotation = projectile.direction.angle()
		projectile.get_node("CPUParticles2D").direction = projectile.direction.normalized().rotated(PI/2)
		get_node("/root/main_game").add_child(projectile)
		i += 1

func knight_special_attack():
	var i = 0
	while i < 35:
		var projectile = load("res://arrow_special_projectile.tscn").instantiate()
		projectile.global_position = Vector2(randf_range(300, 1500), randf_range(-200, -2000))
		projectile.direction = Vector2(randf_range(-0.5, 0.5), randf_range(2, 4)).normalized()
		projectile.damage = 150
		projectile.speed = 400
		projectile.is_player_owned = true
		projectile.spawn_offspring = false
		projectile.time_to_die = 12.0
		projectile.get_node("Sprite2D").rotation = projectile.direction.angle()
		get_node("/root/main_game").add_child(projectile)
		i += 1

func medival_special_attack():
	medival_special_active = true

func miltary_special_attack():
	var plane = load("res://miltary_special_plane.tscn").instantiate()
	plane.global_position = Vector2(-100, 150)
	get_node("/root/main_game").add_child(plane)

func future_special_attack():
	var laser = load("res://future_special_laser_attack.tscn").instantiate()
	get_node("/root/main_game").add_child(laser)
	


func _on_medival_special_timer_timeout():
	medival_special_active = false
	$medival_special_timer.stop()
	for unit in get_node("player_units").get_children():
		if unit.get_node("heal_sprite") != null:
			unit.get_node("heal_sprite").queue_free()

func _on_ai_change_age():
	$enemy_base.update_sprite_ai()


func _on_button_pressed() -> void:
	MusicManager.audioStreamPlayer.stop()
	get_tree().change_scene_to_file("res://scenes/main_menu.tscn")
