using System.Linq.Expressions;
using PlayerStudy.Core.Common;
using PlayerStudy.Core.Logging;

namespace PlayerStudy.Core.StateMachine;

public class StateMachine<TState> : IStateMachine<TState> 
	where TState :IState
{
	private readonly ILogger _logger;
	public TState? CurrentState {get; private set;}
	public StateMachine(ILogger logger)
	{
		_logger = logger;
	}
	public Result ChangeState(TState newState)
	{
		//Check if the new state is a null object.
		if(newState is null)
		{
			Error error = new Error(ErrorCode.InvalidArgument, 
				"Cannot change to null state");

			_logger.Log(LogLevel.Error, error.Message);

			return Result.Failure(error);
		}
		//Check if the new state is same as old state.
		if (ReferenceEquals(CurrentState, newState))
		{
			_logger.Log(
				LogLevel.Debug,
				"State Transition ignored because the requested state is already active."
			);

			return Result.Success();
		}

		TState? prevState = CurrentState;
		Result ret;
		if(prevState is not null)
		{
			try {
			ret = prevState.Exit();
			}
			catch (Exception exception)
			{
				Error error = new Error(ErrorCode.StateExitFailed,
				$"Exception occured while exiting state '{GetStateName(prevState)}'.", exception);
				_logger.Log(LogLevel.Error,error.Message,exception);
				return Result.Failure(error);

			}
			if (!ret.IsSuccess)
			{
				Error error = new Error(ErrorCode.StateExitFailed,
				$"Failed to exit state '{GetStateName(prevState)}'.");
				_logger.Log(LogLevel.Error,error.Message);
				return Result.Failure(error);
			}
		}
		try{
		ret = newState.Enter();
		}
		catch (Exception exception)
		{
			Error error = new Error(ErrorCode.StateEnterFailed,
			$"Exception while entering state '{GetStateName(newState)}'.",exception);
			_logger.Log(LogLevel.Error,error.Message,exception);
			RestorePreviousState(prevState);
			return Result.Failure(error);
		}
		if (!ret.IsSuccess)
		{
			Error error = new Error(ErrorCode.StateEnterFailed,
				$"Failed to enter state '{GetStateName(newState)}'.");
			_logger.Log(LogLevel.Error,error.Message);
			RestorePreviousState(prevState);
			return Result.Failure(error);
		}
		CurrentState = newState;
		
		_logger.Log(
			LogLevel.Debug,
			$"State change from'{GetStateName(prevState)}'to '{newState.GetType().Name}'."
		);
		return Result.Success();
	}
	private void RestorePreviousState(TState? prevState)
	{
		if(prevState is null)
		{
			return;
		}
		Result restoreResult = prevState.Enter();
		if (!restoreResult.IsSuccess)
		{
			_logger.Log(LogLevel.Critical,$"Failed to restore previous state '{GetStateName(prevState)}'.");
		}
	}
	private static string GetStateName(TState? state)
	{
		return state?.GetType().Name ?? "None";
	}
}