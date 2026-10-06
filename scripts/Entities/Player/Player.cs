using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float speed = 250.0f;

	public override void _Ready()
	{
		base._Ready();
		GD.Print("Player is Ready!!");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Vector2 direction = Input.GetVector("Move_left","Move_right","Move_up","Move_down");
		Velocity = direction * speed;
		MoveAndSlide();
	}


}
