extends RigidBody3D

@export var marker: Marker3D
@export var player: Node3D
# Called when the node enters the scene tree for the first time.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	global_position = marker.global_position
	look_at(player.global_position)
