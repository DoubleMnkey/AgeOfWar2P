extends Button


# Called when the node enters the scene tree for the first time.
func _ready():
	$ProgressBar.hide()
	$ProgressBar.value = 100
	$Timer.connect("timeout", _on_timer_timeout)
	$ColorRect.hide()


func _pressed():
	self.disabled = true
	$ProgressBar.show()
	$ProgressBar.value = 0
	$Timer.start(1.0)
	$ColorRect.show()
	
func _on_timer_timeout():
	if $ProgressBar.value != 100:
		$ProgressBar.value += 1
		$Timer.start(1.0)
	else:
		self.disabled = false
		$ProgressBar.hide()
		$Timer.stop()
		$ColorRect.hide()
