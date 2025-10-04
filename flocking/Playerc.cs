using Godot;
using System;

public partial class Playerc : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -750.0f;

	public override void _PhysicsProcess(double delta)
	{
		Timer timer = new Timer();
		timer.WaitTime = 20.0f; 
		timer.OneShot = true;   
		timer.Autostart = true;
		AddChild(timer);

		// Connect the timeout signal
		timer.Connect("timeout", new Callable(this, nameof(OnTimeUp)));
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("move_left", "move_right", "jump", "down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
		
		
	}
	private void OnTimeUp()
	{
		GD.Print("20 seconds passed! Switching scene...");
		GetTree().ChangeSceneToFile("res://Win_menu.tscn");
	}
	
}
