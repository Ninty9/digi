## GDSCRIPT
extends ColorRect

@export var interlace_viewport : SubViewport
@onready var interlacing_material : ShaderMaterial = self.material
@export var interlacing_timer : Timer

####### Connect the interlacing timer's timeout signal here #########
func _on_timer_timeout() -> void:
	var viewport_img : ImageTexture = ImageTexture.create_from_image(
		interlace_viewport.get_texture().get_image()
	)

	interlacing_material.set_shader_parameter("delayed_screen", viewport_img)
	interlacing_timer.start()
