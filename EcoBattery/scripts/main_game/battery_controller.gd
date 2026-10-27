extends Node2D
class_name BatteryController

## 电池控制器 - 电池直线下落，玩家选卡牌设置X轴位置

var energy: int = 0
var is_paused: bool = true
var fall_speed: float = GameData.BATTERY_FALL_SPEED

# 碰撞检测 Y 坐标（对应 main_scene.tscn 中 CardContainer Y=1750, Depot Y=2000）
var card_zone_y_min: float = 1700.0
var card_zone_y_max: float = 1800.0
var depot_zone_y: float = 1950.0

var has_hit_card: bool = false

signal hit_card_zone(energy_count: int)
signal hit_depot_zone(energy_count: int)

@onready var sprite: Sprite2D = $Sprite2D
var energy_bars: Array[Sprite2D] = []


func _ready() -> void:
	# 收集 6 个能量条节点（EnergyBar1 ~ EnergyBar6）
	for i in range(1, 7):
		var bar := get_node_or_null("EnergyBar" + str(i)) as Sprite2D
		if bar:
			energy_bars.append(bar)


func _process(delta: float) -> void:
	if is_paused:
		return

	# 直线下落
	position.y += fall_speed * delta

	# 到达卡牌区域
	if not has_hit_card and position.y >= card_zone_y_min and position.y <= card_zone_y_max:
		has_hit_card = true
		hit_card_zone.emit(energy)

	# 到达仓库区域
	if position.y >= depot_zone_y:
		is_paused = true
		hit_depot_zone.emit(energy)


func setup(fixed_energy: int = -1) -> void:
	is_paused = true
	has_hit_card = false
	if fixed_energy > 0:
		energy = fixed_energy
	else:
		_randomize_energy()
	_update_display()


func start_falling() -> void:
	is_paused = false
	AudioManager.play_se(AudioManager.SE_BATTERY_DROP)


func set_x_pos(x: float) -> void:
	## 瞬间移到卡牌的X轴位置（Unity原版逻辑）
	position.x = x


func reset_to_top(spawn_y: float) -> void:
	position.y = spawn_y
	has_hit_card = false
	_randomize_energy()
	_update_display()


func _randomize_energy() -> void:
	energy = GameData.get_random_battery_energy()


func _update_display() -> void:
	## 对照 Unity: 根据能量值显示对应数量的能量条（从下往上填充）
	for i in energy_bars.size():
		energy_bars[i].visible = i < energy
