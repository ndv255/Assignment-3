extends CharacterBody2D

signal player_action(message: String)
signal stars_changed(count: int)

@export var speed = 300
@export var gravity = 30
@export var jump_force = 500
@export var stars_needed := 3
var stars_collected := 0
@export var speed_boost_multiplier: float = 2.0
@export var speed_boost_duration: float = 3.0
var current_speed: float



func collect_star() -> void:
	stars_collected += 1
	print("Star collected! Total: %d" % stars_collected)
	emit_signal("stars_changed", stars_collected)

	if stars_collected >= stars_needed:
		var wall = get_tree().current_scene.get_node("res://scene/wall.cs")
		if wall:
			wall.queue_free() 
			print("Wall removed! Path is clear.")



func _physics_process(_delta):
	if !is_on_floor():
		velocity.y += gravity
		if velocity.y > 1000:
			velocity.y = 1000


	if Input.is_action_just_pressed("jump") and is_on_floor():
		velocity.y = -jump_force


	var horizontal_direction = Input.get_axis("move_left", "move_right")
	velocity.x = speed * horizontal_direction

	move_and_slide()



	
func win_game():
	print("You Win!")
	emit_signal("player_action", "Player has won the game!")
	
	await get_tree().create_timer(0.1).timeout
	
	get_tree().change_scene_to_file("res://win_menu.tscn")




func _on_teleport_zone_body_entered(body: Node2D) -> void:
	if body == self:  
		var start_point = get_tree().current_scene.get_node("StartPoint")
		if start_point:
			global_position = start_point.global_position
			velocity = Vector2.ZERO 
		get_tree().change_scene_to_file("res://lose_menu.tscn")


func _on_jump_pad_body_entered(body: Node2D) -> void:
	if body == self:  
		velocity.y = -jump_force * 2


	
var original_speed: int

func _ready():
	original_speed = speed
	var speed_boost = get_node("res://speed_boost.cs")
	if speed_boost:
		speed_boost.connect("SpeedBoosted", Callable(self, "_on_speed_boosted"))

func _on_speed_boosted():
	speed = original_speed * speed_boost_multiplier
	var timer = Timer.new()
	timer.wait_time = speed_boost_duration
	timer.one_shot = true
	timer.autostart = true
	add_child(timer)
	timer.timeout.connect(_reset_speed)

func _on_speed_boost_body_entered(body: Node2D) -> void:
	if body != self:
		return


	speed = original_speed * speed_boost_multiplier

	var timer = Timer.new()
	timer.wait_time = speed_boost_duration
	timer.one_shot = true
	timer.autostart = true
	add_child(timer)
	timer.timeout.connect(_reset_speed)

func _reset_speed():
	speed = original_speed
