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
	[Export] private RichTextLabel cha;
	[Export] private TextureRect img;
	[Export] private VBoxContainer optionContianer;
	private RichTextLabel[] optionLabels;
	
	[Export] private Profile[] profiles;

	private List<string> inputs = new ();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		optionLabels = new RichTextLabel[5];
		int i = 0;
		foreach (var n in optionContianer.GetChildren())
			optionLabels[i++] = n.GetNode<RichTextLabel>(".");
		
		d = this;
	}
	
	public void OnDialogueOutput(String dialogue, String character, Array parameters)
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

	public override void _PhysicsProcess(double delta)
	{
		// CheckInput();
	}

	private void CheckInput()
	{
		if(CurrentTree == null) return;
		if(inputs.Count != 0) return;
		if(CurrentTree.GetInputOptions().Length == 0) return;
		inputs = new List<string>();
		foreach (var input in CurrentTree.GetInputOptions())
		{
			if (input.Input == "") break;
			inputs.Add(input.Input);
		}
		if(inputs.Count == 0) return;
		GD.Print("banzingr");
		string options = "";
		foreach (var i in inputs)
		{
			if (options == "")
				options += i;
			else
				options += "\n" + i;

		}
		OnDialogueOutput(options, "bean", null);
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