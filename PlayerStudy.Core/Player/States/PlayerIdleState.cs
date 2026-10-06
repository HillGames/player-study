using PlayerStudy.Core.Common;
using PlayerStudy.Core.StateMachine;

namespace PlayerStudy.Core.Player.States;

public sealed class PlayerIdleState : IState
{
	public Result Enter()
	{
		return Result.Success();
	}
	public Result Exit()
	{
		return Result.Success();
	}
}