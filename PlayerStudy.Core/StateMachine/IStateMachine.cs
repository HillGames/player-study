using PlayerStudy.Core.Common;
namespace PlayerStudy.Core.StateMachine;
public interface IStateMachine<TState> where TState : IState
{
	TState? CurrentState {get;}
	Result ChangeState(TState newState);
}