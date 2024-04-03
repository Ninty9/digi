using Godot;
using System;
using System.Numerics;
using digi;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Quaternion = Godot.Quaternion;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public partial class Camera : CharacterBody3D
{
	[Export] private Node3D target;

	[Export] private float distance;
	[Export] private float focusRadius;
	[Export] private float sens;
	[Export] private Resource shape;
	[Export] private CharacterBody3D camTarg;
	Vector3 focusPoint;

	private Vector3 rotVec;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rotVec = Vector3.Forward * distance;
		focusPoint = target.GlobalPosition;
		camTarg.GlobalPosition = focusPoint;
		GD.Print(focusPoint);
		// camTarg.MoveAndCollide( focusPoint - Vector3.Forward * distance);
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		UpdateFocusPoint();
		if (camTarg.MoveAndCollide((focusPoint + rotVec) - camTarg.GlobalPosition) != null)
		{
			camTarg.Velocity = (focusPoint + rotVec) - camTarg.GlobalPosition;
			camTarg.MoveAndSlide();
		}
		Velocity = (camTarg.GlobalPosition - GlobalPosition) * 8;
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		LookAt(target.GlobalPosition);
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

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouse)
		{
			mouse.Relative *= sens;
			// mouse.Relative = mouse.Relative.Clamp(new Vector2(-15, -15), new Vector2(15, 15));
			rotVec = RotateVector(-(focusPoint - camTarg.GlobalPosition), mouse.Relative);
		}

		if (Input.IsKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
	}
	
	private Vector3 RotateVector(Vector3 vector, Vector2 angle)
	{
		// Convert angles to radians
		float xRad = Mathf.DegToRad(angle.Y);
		float yRad = Mathf.DegToRad(angle.X);
		vector = vector.Normalized();
		// Rotate around x axis
		vector = vector.Rotated(vector.Cross(new Vector3(0, 1, 0)).Normalized(), -xRad);

		// Rotate around y axis
		vector = vector.Rotated(new Vector3(0, 1, 0), yRad);

		return vector * distance;
	}
}
