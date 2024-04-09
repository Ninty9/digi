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
	
	public static bool WantsInput;
	private static bool choosing;
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
	
	public void OnDialogueOutput(List<string> dialogue, string character, Array parameters)
	{
		string newD = "";
		foreach (var i in dialogue)
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
		if (Input.IsActionJustReleased("Interact"))
		{
			if(choosing)
			{
				choosing = false;
				WantsInput = false;
				diaSel.Visible = false;
				CurrentTree.SendInput(inputs[(int)Mathf.Floor(diaSel.Position.Y / (dia.Size.Y / inputs.Count))]);
				inputs = new List<string>();
				return;
			}
			if (WantsInput)
			{
				choosing = true;
				return;
			}

		}
		GD.Print((int)Mathf.Floor(diaSel.Position.Y / (dia.Size.Y / inputs.Count)));
		for (int i = 0; i < inputs.Count; i++)
		{
			GD.Print(i + ": " + inputs[i]);
		}
		if (Input.IsActionJustPressed("Interact") && choosing)
		{
			diaSel.Visible = true;
		}
		if (Input.IsActionPressed("Interact") && choosing)
		{
			diaSel.Position = diaSel.Position with
			{
				Y = (diaSel.Position.Y + selSpeed * (float)delta) % (dia.Size.Y - 10)
			};
		}

	}

	public override void _PhysicsProcess(double delta)
	{
		CheckInput();
	}

	private void CheckInput()
	{
		if (CurrentTree == null) return;
		if(inputs.Count != 0) return;
		if(CurrentTree.GetInputOptions().Length == 0) return;
		inputs = new List<string>();
		foreach (var input in CurrentTree.GetInputOptions())
		{
			if (input.Input == "") break;
			inputs.Add(input.Input);
		}
		if(inputs.Count == 0) return;

		OnDialogueOutput(inputs, "Bean", null);
		WantsInput = true;
	}
	
	public static void Start(DialogueTree tree)
	{
		if(CurrentTree != null)
		{
			End();
		}
		CurrentTree = tree;
		CurrentTree.DialogueOutput += (dialogue, character, parameters) =>
			d.OnDialogueOutput(dialogue, character, parameters);
		CurrentTree.DialogueEnded += End;
		CurrentTree.StartDialogue();
		d.Visible = true;
	}
	

	public static void End()
	{
		CurrentTree.DialogueOutput -= d.OnDialogueOutput;
		CurrentTree.DialogueEnded -= End;
		CurrentTree = null;
		d.Visible = false;
	}
}