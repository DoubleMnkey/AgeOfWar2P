@tool
extends Control

signal pressed

@export_category("Settings")
@export var text: String = "NEW BUTTON"
@export var font_size: int = 26
@export var hover_pointer_offset: float = 30

@export_category("ButtonElements")
@export var button: Button
@export var label_background_nodes: Array[Label]
@export var label_foreground_node: Label
@export var show_hover_pointer: bool = true
@export var hover_pointer: Control
@export var dark: bool = false

var editor_update_previous_text: String

func _ready() -> void:
	_on_button_exited()
	set_text(text)
	
	if dark:
		for label_node in label_background_nodes:
			label_node.visible = false
		label_foreground_node.label_settings.font_color = Color(0, 0, 0, 1)
		
	
	button.connect("mouse_entered", _on_button_enter)
	button.connect("mouse_exited", _on_button_exited)
	button.connect("pressed", _on_button_pressed)

func _process(delta: float) -> void:
	if not Engine.is_editor_hint():
		return
	
	if text != editor_update_previous_text:
		editor_update_previous_text = text
		set_text(text)

func set_text(text: String) -> void:
	for label_node in label_background_nodes:
		label_node.label_settings = label_node.label_settings.duplicate()
		label_node.text = text
		label_node.label_settings.font_size = font_size
	
	label_foreground_node.label_settings = label_foreground_node.label_settings.duplicate()
	label_foreground_node.text = text
	label_foreground_node.label_settings.font_size = font_size
	
	var text_width = label_foreground_node.label_settings.font.get_string_size(label_foreground_node.text, HORIZONTAL_ALIGNMENT_CENTER, -1, label_foreground_node.label_settings.font_size).x
	hover_pointer.position.x = (position.x + size.x / 4.0) - text_width / 2.0 + (hover_pointer.size.x * hover_pointer.scale.x) / 2.0 - hover_pointer_offset

func _on_button_enter():
	if not show_hover_pointer:
		_on_button_exited()
		return
	
	hover_pointer.visible = true

func _on_button_exited():
	hover_pointer.visible = false

func _on_button_pressed():
	pressed.emit()
