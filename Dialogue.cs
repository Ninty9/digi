using System;
using System.Collections.Generic;
using System.Linq;
using Ardot.DialogueTrees;
using Godot;
using Array = Godot.Collections.Array;


namespace digi;

public partial class Dialogue : Control
{
	private static Dialogue d;
	public static DialogueTree CurrentTree;

	[Export] private RichTextLabel dia;
	[Export] private ColorRect diaSel;
	[Export] private float selSpeed;
	[Export] private RichTextLabel cha;
	[Export] private TextureRect img;
	[Export] private Profile[] profiles;

	private string[] options = new string[6];
	
	public static bool WantsInput;
	private float selHeight;
	
	private List<string> inputs = new ();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		d = this;
	}
	
	public void OnDialogueOutput(string dialogue, string character, Array parameters)
	{
		Visible = true;
		dia.Text = dialogue;
		cha.Text = character;
		foreach (Profile p in d.profiles)
		{
			if (p.Name == character)
			{
				d.img.Texture = p.Image;
			}
		}
	}
	
	public void OnDialogueOutput(string[] dialogue, string character, Array parameters)
	{
		string newD = "";
		foreach (var i in inputs)
		{
			if (newD == "")
				newD += i;
			else
				newD += "\n" + i;

		}
		OnDialogueOutput(newD, character, parameters);
	}

	public override void _Process(double delta)
	{
		if(!WantsInput) return;
		if (Input.IsActionJustPressed("Interact"))
		{
			diaSel.Visible = true;
			GD.Print("bint");
		}
		if (Input.IsActionPressed("Interact"))
		{
			GD.Print("ed");
			diaSel.Position = diaSel.Position with
			{
				Y = (diaSel.Position.Y + selSpeed * (float)delta) % (dia.Size.Y - 10)
			};
		}
		if (Input.IsActionJustReleased("Interact"))
		{
			// WantsInput = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		CheckInput();
	}

	private void CheckInput()
	{
		if(CurrentTree == null) return;
		GD.Print(CurrentTree.GetInputOptions().Length);
		if(inputs.Count != 0) return;
		if(CurrentTree.GetInputOptions().Length == 0) return;
		inputs = new List<string>();
		foreach (var input in CurrentTree.GetInputOptions())
		{
			if (input.Input == "") break;
			inputs.Add(input.Input);
		}
		if(inputs.Count == 0) return;
		for (var i = 0; i < inputs.Count; i++)
		{
			options[i] = inputs[i];
		}

		OnDialogueOutput(options, "bean", null);
		WantsInput = true;
	}
	
	public static void Start(DialogueTree tree)
	{
		if(CurrentTree != null) CurrentTree.DialogueOutput -= d.OnDialogueOutput;
		CurrentTree = tree;
		CurrentTree.DialogueOutput += (dialogue, character, parameters) =>
			d.OnDialogueOutput(dialogue, character, parameters);
		CurrentTree.StartDialogue();
		d.Visible = true;
	}
	

	public static void End()
	{
		CurrentTree = null;
		d.Visible = false;
	}
}