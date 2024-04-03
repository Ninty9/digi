using System;
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

	[Export] private Profile[] profiles;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		d = this;
	}
	

	public void OnDialogueOutput(String dialogue, String character, Array parameters)
	{
		GD.Print("binted");
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