using PlayerStudy.Core.Common;

namespace PlayerStudy.Core.StateMachine;
public interface IState
{
	Result Enter();
	Result Exit();
}