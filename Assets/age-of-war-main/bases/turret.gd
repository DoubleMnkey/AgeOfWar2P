extends Node2D
class_name turret

@export var is_player_owned: bool = true

var projectile_damge
var projectile_speed
var projectile_texture
var spawn_projectile_offspring : bool
var offspring_texture
var projectile_rotation : bool

var target_queue: Array
var current_target

var spawn_projectile_frame
var spawn_projectile_frame2
var play_audio_frame
var play_audio_frame2
var offset: Vector2 = Vector2(0,0)
var no_rotation: bool
var play_audio_repeated: bool

var spawn_explosion_effect : bool


# Called when the node enters the scene tree for the first time.
func _ready():
	for child in get_node("sfx").get_children():
		child.bus = &'sfx'
	
	$range.connect("body_entered", _on_range_body_entered)
	no_rotation = false
	z_index = 1

	if is_player_owned == false:
		self.scale.x = -1


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if target_queue.size() > 0:
		current_target = target_queue[0]
		# print(current_target.get_node("CollisionShape2D").global_position)
		if current_target == null or current_target.get_node("CollisionShape2D").disabled == true:
			target_queue.pop_front()
			return
		
		var target_position: Vector2 = current_target.get_node("CollisionShape2D").global_position - self.global_position - self.offset
				
		if $AnimatedSprite2D.get_animation() == "idle":
			$AnimatedSprite2D.play("attack")
			
		# self.global_position.angle_to_point(current_target.get_node("CollisionShape2D").global_position - self.global_position)
		if no_rotation == false:
			if is_player_owned == false:
				var angle = target_position.reflect(Vector2(0,1)).angle()
				$AnimatedSprite2D.rotation = angle
			else: # Turret is owned by player
				$AnimatedSprite2D.rotation = target_position.angle()
		
		if $AnimatedSprite2D.frame == spawn_projectile_frame or $AnimatedSprite2D.frame == spawn_projectile_frame2:
			spawn_projectile(target_position.normalized(), projectile_texture)
		elif ($AnimatedSprite2D.frame == play_audio_frame or $AnimatedSprite2D.frame == play_audio_frame2) and ($sfx/AudioStreamPlayer2D.is_playing() == false or play_audio_repeated == true):
			$sfx/AudioStreamPlayer2D.play()
	else:
		# TODO wait for attack animation to finish before moving to idle state
		$AnimatedSprite2D.play("idle")




func _on_range_body_entered(body):
	if body is melee_unit or body is range_unit:
		if body.is_player_owned != is_player_owned:
			target_queue.append(body)

func spawn_projectile(direction, texture):
	var projectile = load("res://projectile.tscn").instantiate()
	projectile.global_position = self.global_position + offset
	projectile.direction = direction
	projectile.damage = projectile_damge
	projectile.speed = projectile_speed
	projectile.is_player_owned = self.is_player_owned
	projectile.spawn_offspring = spawn_projectile_offspring
	projectile.get_node("Sprite2D").texture = texture
	if self.is_player_owned == false:
		projectile.scale.x = -1
	projectile.get_node("Sprite2D").rotation = $AnimatedSprite2D.rotation
	if projectile_rotation == true:
		projectile.rotation_speed = randf_range(-100.0, 100.0)
	if offspring_texture == null:
		projectile.offspring_texture = projectile_texture
	else:
		projectile.offspring_texture = offspring_texture
	projectile.spawn_explosion_effect = spawn_explosion_effect
	get_node("/root/main_game").add_child(projectile)
