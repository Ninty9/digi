using System.Diagnostics;
using System.Linq;
using Godot;

namespace digi;

public partial class Player : RigidBody3D
{
	[Export] private Label3D face;
	private Vector2 cam;

	[Export] private Node3D camera;

	[Export] private AudioStreamPlayer3D hitSound;

	[Export] private Area3D interactCollider;
	
	private Vector3 velocity;
	
	private const float Acceleration = 20f;
	private const float Deceleration = 10f;
	private const float Speed = 9f;

	private double lastTouched;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		lastTouched += delta;
		SetFace();
		Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");
		Vector3 direction = (camera.Basis * new Vector3(inputDir.Y, 0, -inputDir.X)).Normalized();
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
		ApplyTorque(velocity);

	}

	public void _on_collision(Node3D body)
	{
		hitSound.Play();
		lastTouched = 0;
	}

	public void SetFace()
	{
		if (lastTouched < 0.1f)
		{
			face.Text = "):<";
			return;
		}
		if (Input.IsAnythingPressed())
		{
			face.Text = ">_<";
			return;
		}
		if (Mathf.Round(AngularVelocity.Length()) > 1)
		{
			face.Text = "O:";
			return;
		}
		face.Text = "(:";
		
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Interact"))
		{
			if(Dialogue.CurrentTree != null && !Dialogue.WantsInput)
			{
				Dialogue.CurrentTree.SendInput();
				return;
			}
			interactCollider.GetOverlappingAreas().First().Call("Call");
		}
	}
	
}