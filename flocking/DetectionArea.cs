using Godot;
using System;

public partial class DetectionArea : Area2D
{
	[Export] public string SceneToLoad = "res://NewScene.tscn";

	public override void _Ready()
	{
		// Connect the body_entered signal
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	private void OnBodyEntered(Node2D body)
	{
		// Check if the object entering is a boid
		if (body is Boid)
		{
			GetTree().ChangeSceneToFile("res://lose_menu.tscn");
		}
	}
}
