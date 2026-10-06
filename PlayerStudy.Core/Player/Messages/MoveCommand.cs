using PlayerStudy.Core.Common.Math;
using PlayerStudy.Core.Messaging;

namespace PlayerStudy.Core.Player.Messages;

public sealed class MoveCommand : ICommand
{
	public Vector2 Input {get;}
	public MoveCommand(Vector2 input)
	{
		Input = input;
	}
}