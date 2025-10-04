using Godot;
using System;

public partial class Target1 : CharacterBody2D
{
	private NavigationAgent2D _navigationAgent;
	private Vector2 _startPosition;
	private float _movementSpeed = 200.0f;
	private Vector2 _movementTargetPosition = new Vector2(200.0f, 300.0f);
	
	public Vector2 MovementTarget
	{
		get { return _navigationAgent.TargetPosition; }
		set { _navigationAgent.TargetPosition = value; }
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		_navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		// These values need to be adjusted for the actor's speed
		// and the navigation layout.
		_navigationAgent.PathDesiredDistance = 4.0f;
		_navigationAgent.TargetDesiredDistance = 4.0f;
		_startPosition = GlobalPosition;
		// Make sure to not await during _Ready.
		Callable.From(ActorSetup).CallDeferred();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		//Vector2 v = GetPosition();
		base._PhysicsProcess(delta);

		if (_navigationAgent.IsNavigationFinished())
		{
			ResetPathPosition();
			_startPosition = GlobalPosition;
			return;
		}

		Vector2 currentAgentPosition = GlobalTransform.Origin;
		Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

		Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * _movementSpeed;
		MoveAndSlide();
	}
	public void ResetPathPosition()
	{
		MovementTarget = _startPosition;
	}
	
	private async void ActorSetup()
	{
		// Wait for the first physics frame so the NavigationServer can sync.
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

		// Now that the navigation map is no longer empty, set the movement target.
		MovementTarget = _movementTargetPosition;
	}
}
