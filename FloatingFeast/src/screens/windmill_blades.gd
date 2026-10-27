extends TextureRect
## Spins the deck windmill's sails, and the gear train inside the windmill.
##
## The hub pivot is set in the scene, not here: a Control scales around pivot_offset as well as
## rotating around it, so moving the pivot to the centre shifts the node by pivot × (scale − 1).
## Doing that from _ready would leave the editor showing one position and the game rendering
## another — so the scene carries both the pivot and the matching position offset.

## Degrees per second; negative reverses. Slow reads better on pixel art — fast spin smears the
## blades. Meshing gears must alternate sign, and equal-tooth gears must share a magnitude, or the
## teeth visibly slide through each other at the contact point.
@export var degrees_per_second: float = 20.0

func _process(delta: float) -> void:
	rotation_degrees = fmod(rotation_degrees + degrees_per_second * delta, 360.0)
