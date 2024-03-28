using Godot;
using System;

public partial class Camera : RigidBody3D
{
	[Export] private Node3D target;

	[Export] private float distance;
	[Export] private float focusRadius;
	Vector3 focusPoint;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		focusPoint = target.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateFocusPoint();
		Vector3 lookDirection = Vector3.Forward;

		Vector3 targetPos = focusPoint - lookDirection * distance;
		ApplyForce(targetPos - GlobalPosition);
	}

	private void UpdateFocusPoint()
	{
		Vector3 targetPoint = target.GlobalPosition;
		if (focusRadius > 0f) {
			float dist = targetPoint.DistanceTo(focusPoint);
			if (dist > focusRadius) {
				focusPoint = targetPoint.Lerp(
					focusPoint, focusRadius / dist
				);
			}
		}
		else {
			focusPoint = targetPoint;
		}
	}
}
