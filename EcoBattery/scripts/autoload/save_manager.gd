extends Node

## 存档管理器 - 负责游戏数据的保存和加载

const SAVE_PATH: String = "user://save_data.json"

var coin: int = 0
var best_record: int = 0


func _ready() -> void:
	load_data()


func _notification(what: int) -> void:
	if what == NOTIFICATION_WM_CLOSE_REQUEST:
		save_data()


func save_data() -> void:
	var data: Dictionary = {
		"coin": coin,
		"best_record": best_record,
	}
	var file := FileAccess.open(SAVE_PATH, FileAccess.WRITE)
	if file:
		file.store_string(JSON.stringify(data))


func load_data() -> void:
	if not FileAccess.file_exists(SAVE_PATH):
		return
	var file := FileAccess.open(SAVE_PATH, FileAccess.READ)
	if file:
		var json := JSON.new()
		var result := json.parse(file.get_as_text())
		if result == OK:
			var data: Dictionary = json.data
			coin = data.get("coin", 0)
			best_record = data.get("best_record", 0)


func add_coin(amount: int) -> void:
	coin += amount
	save_data()


func spend_coin(amount: int) -> bool:
	if coin >= amount:
		coin -= amount
		save_data()
		return true
	return false


func update_best_record(record: int) -> void:
	if record > best_record:
		best_record = record
		save_data()
