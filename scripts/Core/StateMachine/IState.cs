namespace Game.Core.StateMachine;

public interface IState
{
	void Enter();
	void Exit();
	void Update(double delta);
}