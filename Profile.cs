using Godot;

namespace digi;

[GlobalClass]
public partial class Profile : Resource
{
    [Export] public Texture2D Image;
    [Export] public string Name;
}