extends Sprite2D
class_name EnergyCell

## 能量格子 - 显示单个能量格的填充状态

@export var filled_texture: Texture2D
@export var empty_texture: Texture2D

var is_filled: bool = false


func set_filled(filled: bool) -> void:
	is_filled = filled
	if filled and filled_texture:
		texture = filled_texture
	elif not filled and empty_texture:
		texture = empty_texture
