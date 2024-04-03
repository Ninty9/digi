using Ardot.DialogueTrees;
using Godot;

namespace digi;


[GlobalClass]
public partial class TalkInteract : Area3D
{
    [Export] private DialogueTree tree;
    public void Call()
    {
        Dialogue.Start(tree);
    }

    public void _on_area_exited(Area3D area)
    {
        if (area.HasMeta("isPlayer") && Dialogue.CurrentTree == tree)
        {
            Dialogue.End();
        }
    }
}