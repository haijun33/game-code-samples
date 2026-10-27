class_name Ember extends AnimatedSprite2D
## Ember, the hearth-spirit that floats at Saff's shoulder. Drop this scene into any walkable screen
## and it finds the player and tags along; scenes that steer Ember themselves (the cave leans her
## toward a deposit while appraising) turn follow_player off and drive the position directly.

const SHOULDER := Vector2(40, -6) # beside her right shoulder — the sprite is 52x48, so the
								  # old blob-sized offset parked her on top of the chef hat
const FOLLOW := 6.0 # lerp rate; low enough that she trails rather than sticks
const BOB_AMP := 3.0

## Off when the owning scene positions Ember itself.
@export var follow_player: bool = true

var _player: Node2D = null
var _bob := 0.0


func _ready() -> void:
	if not follow_player:
		return
	_player = _find_player()
	if _player != null:
		global_position = _player.global_position + SHOULDER


func _process(delta: float) -> void:
	if not follow_player or _player == null:
		return
	_bob += delta * 3.0
	var target := _player.global_position + SHOULDER + Vector2(0.0, sin(_bob) * BOB_AMP)
	global_position = global_position.lerp(target, clampf(delta * FOLLOW, 0.0, 1.0))


## Scoped to this scene rather than a plain group lookup: SceneRouter frees the outgoing screen and
## adds the incoming one in the same frame, so both players are briefly in the tree and a bare lookup
## can latch onto the one that is about to die.
func _find_player() -> Node2D:
	var scene := owner
	for n in get_tree().get_nodes_in_group(&"player"):
		if n is Node2D and scene != null and scene.is_ancestor_of(n):
			return n
	return null
