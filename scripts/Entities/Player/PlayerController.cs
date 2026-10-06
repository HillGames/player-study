using Godot;
using System;

public partial class PlayerController : Node
{
	private Player _player;
	public override void _Ready()
	{
		_player = GetParent<Player>();

	}

	public void Move(Vector2 direction)
	{
		_player.Velocity = direction * _player.speed;
		_player.MoveAndSlide();
	}
}
