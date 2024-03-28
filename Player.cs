using System.Diagnostics;
using Godot;

namespace digi;

public partial class Player : RigidBody3D
{
	[Export] private float camDistance;
	[Export] private float sens;
	private Vector2 cam;

	[Export] private Marker3D camera;

	private Vector3 velocity;
	
	private const float Acceleration = 20f;
	private const float Deceleration = 10f;
	private const float Speed = 9f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{

			Vector3 targetVel = direction * Speed;
			velocity.X = (float)Mathf.MoveToward(velocity.X, targetVel.X, Acceleration * delta);
			velocity.Z = (float)Mathf.MoveToward(velocity.Z, targetVel.Z, Acceleration * delta);
		}
		else
		{
			velocity.X = (float)Mathf.MoveToward(velocity.X, 0, Deceleration * delta);
			velocity.Z = (float)Mathf.MoveToward(velocity.Z, 0, Deceleration * delta);
		}
		GD.Print(velocity);
		ApplyTorque(velocity);
	}
}