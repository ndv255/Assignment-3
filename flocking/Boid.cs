using Godot;
using System.Collections.Generic;

public partial class Boid : CharacterBody2D
{

	[Export] public float MaxSpeed = 500.0f;
	[Export] public float SeparationWeight = 1.0f;
	[Export] public float AlignmentWeight = 1.0f;
	[Export] public float CohesionWeight = 1.0f;
	[Export] public float FollowWeight = 1.0f;
	[Export] public float FollowRadius = 200.0f;
	[Export] public float SeparationDistance = 100.0f;
	private List<Boid> _neighbors = new();
	private Area2D _detectionArea;
	private CharacterBody2D _target;
	public override void _Ready()
	{
		_detectionArea = GetNode<Area2D>("DetectionArea");
		_detectionArea.BodyEntered += OnBodyEntered;
		_detectionArea.BodyExited += OnBodyExited;
		var viewportRect = GetViewportRect();
		var randomX = GD.Randf() * viewportRect.Size.X;
		var randomY = GD.Randf() * viewportRect.Size.Y;
		Position = new Vector2(randomX, randomY);
		
		var randomAngle = GD.Randf() * Mathf.Pi * 2;
		var randomVelocity = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * MaxSpeed;
		Velocity = randomVelocity;
		MoveAndSlide();
		
		
		LookAt(Position + Velocity);
	}
	public void SetTarget(CharacterBody2D Target)
	{
		_target = Target;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 separationVector = Separation() * SeparationWeight;
		
		Vector2 alignmentVector = Alignment() * AlignmentWeight;
		Vector2 cohesionVector = Cohesion() * CohesionWeight;
		Vector2 followVector = Centralization() * FollowWeight;
		Vector2 direction = (separationVector + alignmentVector + cohesionVector + followVector).Normalized();
		
		Velocity = Velocity.Lerp(direction*MaxSpeed, (float)delta);
		
	
		MoveAndSlide();
		
		
		LookAt(Position + Velocity);
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Boid boid && body != this)
		{
			_neighbors.Add(boid);
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body is Boid boid && body != this)
		{
			_neighbors.Remove(boid);
		}
	}

	
	private Vector2 Separation()
	{
		if (_neighbors.Count == 0) return Vector2.Zero;

		Vector2 steer = Vector2.Zero;
		foreach (var neighbor in _neighbors)
		{
			Vector2 diff = Position - neighbor.Position;
			if (diff.Length() < SeparationDistance)
				steer += diff / diff.Length();
		}
		
		return steer.Normalized();
	}

	
	private Vector2 Alignment()
	{
		if (_neighbors.Count == 0) return Vector2.Zero;

		Vector2 averageVelocity = Vector2.Zero;
		foreach (var neighbor in _neighbors)
		{
			averageVelocity += neighbor.Velocity;
		}
		averageVelocity /= _neighbors.Count;
		return averageVelocity.Normalized();
	}

	
	private Vector2 Cohesion()
	{
		if (_neighbors.Count == 0) return Vector2.Zero;

		Vector2 centerOfMass = Vector2.Zero;
		foreach (var neighbor in _neighbors)
		{
			centerOfMass += neighbor.Position;
		}
		centerOfMass /= _neighbors.Count;

		Vector2 directionToCenter = centerOfMass - Position;
		return directionToCenter.Normalized();
	}
	
	private Vector2 Centralization()
	{
		if(_target != null)
		{
			
			if (Position.DistanceTo(_target.GetPosition()) < FollowRadius)
			{
				return Vector2.Zero;
			}
			else
			{
				return ((_target.GetPosition() - Position).Normalized());
			}
		}
		else
		{
			return Vector2.Zero;
		}
		
	}
	private void BoidExitedScreen()
	{
		
		Velocity *= -1;
		MoveAndSlide();
		
		
		LookAt(Position + Velocity);
	}
}
