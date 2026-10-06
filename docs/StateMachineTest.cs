using System.Runtime;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using NUnit.Framework;
using PlayerStudy.Core.Common;
using PlayerStudy.Core.Logging;
using PlayerStudy.Core.StateMachine;

namespace PlayerStudy.Test.StateMachine;

[TestFixture]
public class StateMachineTest
{
	private TestLogger _logger = null!;
	private StateMachine<TestState> _stateMachine = null!;

	[SetUp]
	public void SetUp()
	{
		_logger = new TestLogger();
		_stateMachine = new StateMachine<TestState>(_logger);
	}
	[Test]
	public void NewStateMachine_ShouldhaveNoCurrentState()
	{
		Assert.That(_stateMachine.CurrentState, Is.Null);
	}

	[Test]
	public void StateMachine_ChangeState_ShouldEnterState()
	{
		TestState state = new TestState();
		Result result = _stateMachine.ChangeState(state);
		Assert.That(result.IsSuccess, Is.True);
		Assert.That(result.Error,Is.Null);
		Assert.That(_stateMachine.CurrentState,Is.SameAs(state));
		Assert.That(state.EnterCount,Is.EqualTo(1));
		Assert.That(state.ExitCount,Is.EqualTo(0));

	}

}

internal sealed class TestState : IState
{
	public int EnterCount {get; private set;}
	public int ExitCount {get; private set;}

	public Result Enter()
	{
		EnterCount++;
		return Result.Success();
	}

	public Result Exit()
	{
		ExitCount++;
		return Result.Success();
	}
}
internal sealed class TestLogger : ILogger
{
	public List<LogEntry> Entries {get;} = new();
	public void Log(LogLevel level,
					string message)
	{
		Entries.Add(new LogEntry(level,message));
	}
	public void Log(LogLevel level,
					string message,
					Exception? exception)
	{
		Entries.Add(new LogEntry(level,message,exception));
	}

}
internal sealed record LogEntry(LogLevel level,
	string message,
	Exception? exception = null);
