
using NUnit.Framework;
using PlayerStudy.Core.Common;
using PlayerStudy.Core.Logging;
using PlayerStudy.Core.StateMachine;

namespace PlayerStudy.Test.StateMachine;

[TestFixture]
public sealed class StateMachineTest
{
    private TestLogger _logger = null!;
    private StateMachine<TestState> _stateMachine = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = new TestLogger();

        _stateMachine = new StateMachine<TestState>(
            _logger);
    }

    // ------------------------------------------------------------------------
    // Initialization
    // ------------------------------------------------------------------------

    [Test]
    public void Constructor_ShouldStartWithNoCurrentState()
    {
        Assert.That(
            _stateMachine.CurrentState,
            Is.Null,
            "A newly created state machine must not have an active state.");
    }

    // ------------------------------------------------------------------------
    // First transition
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_FirstState_ShouldEnterState()
    {
        TestState state = new TestState("StateA");

        Result result = _stateMachine.ChangeState(state);

        Assert.Multiple(() =>
        {
            Assert.That(
                result.IsSuccess,
                Is.True,
                "Changing to the first state should succeed.");

            Assert.That(
                result.Error,
                Is.Null,
                "A successful transition should not contain an error.");

            Assert.That(
                _stateMachine.CurrentState,
                Is.SameAs(state),
                "The requested state should become the current state.");

            Assert.That(
                state.EnterCount,
                Is.EqualTo(1),
                "The new state must be entered exactly once.");

            Assert.That(
                state.ExitCount,
                Is.EqualTo(0),
                "The first state must not be exited.");
        });
    }

    // ------------------------------------------------------------------------
    // Normal transition
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_ShouldExitPreviousStateAndEnterNewState()
    {
        TestState firstState = new TestState("StateA");
        TestState secondState = new TestState("StateB");

        Result firstResult = _stateMachine.ChangeState(firstState);

        Assert.That(
            firstResult.IsSuccess,
            Is.True,
            "Initial state transition must succeed.");

        Result secondResult = _stateMachine.ChangeState(secondState);

        Assert.Multiple(() =>
        {
            Assert.That(
                secondResult.IsSuccess,
                Is.True,
                "A normal state transition should succeed.");

            Assert.That(
                secondResult.Error,
                Is.Null);

            Assert.That(
                firstState.ExitCount,
                Is.EqualTo(1),
                "The previous state must be exited exactly once.");

            Assert.That(
                secondState.EnterCount,
                Is.EqualTo(1),
                "The new state must be entered exactly once.");

            Assert.That(
                secondState.ExitCount,
                Is.EqualTo(0),
                "The newly entered state must not be exited.");

            Assert.That(
                _stateMachine.CurrentState,
                Is.SameAs(secondState),
                "The new state must become the current state.");
        });
    }

    // ------------------------------------------------------------------------
    // Same-state transition
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_SameState_ShouldDoNothing()
    {
        TestState state = new TestState("StateA");

        Result firstResult = _stateMachine.ChangeState(state);

        Assert.That(
            firstResult.IsSuccess,
            Is.True);

        int enterCountBefore = state.EnterCount;
        int exitCountBefore = state.ExitCount;

        Result secondResult = _stateMachine.ChangeState(state);

        Assert.Multiple(() =>
        {
            Assert.That(
                secondResult.IsSuccess,
                Is.True,
                "Requesting the already-active state should succeed.");

            Assert.That(
                state.EnterCount,
                Is.EqualTo(enterCountBefore),
                "The state must not be entered again.");

            Assert.That(
                state.ExitCount,
                Is.EqualTo(exitCountBefore),
                "The state must not be exited.");

            Assert.That(
                _stateMachine.CurrentState,
                Is.SameAs(state));
        });
    }

    // ------------------------------------------------------------------------
    // Transition ordering
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_ShouldExitPreviousStateBeforeEnteringNewState()
    {
        TestState firstState = new TestState("StateA");
        TestState secondState = new TestState("StateB");

        List<string> lifecycleOrder = new();

        firstState.OnEnter = () =>
        {
            lifecycleOrder.Add("StateA.Enter");
        };

        firstState.OnExit = () =>
        {
            lifecycleOrder.Add("StateA.Exit");
        };

        secondState.OnEnter = () =>
        {
            lifecycleOrder.Add("StateB.Enter");
        };

        secondState.OnExit = () =>
        {
            lifecycleOrder.Add("StateB.Exit");
        };

        Result firstResult = _stateMachine.ChangeState(firstState);

        Assert.That(firstResult.IsSuccess, Is.True);

        lifecycleOrder.Clear();

        Result secondResult = _stateMachine.ChangeState(secondState);

        Assert.That(
            secondResult.IsSuccess,
            Is.True);

        Assert.That(
            lifecycleOrder,
            Is.EqualTo(new[]
            {
                "StateA.Exit",
                "StateB.Enter"
            }),
            "The previous state must exit before the new state enters.");
    }

    // ------------------------------------------------------------------------
    // Exit failure
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_WhenExitFails_ShouldRejectTransition()
    {
        TestState firstState = new TestState("StateA");
        TestState secondState = new TestState("StateB");

        firstState.ExitResult = Result.Failure(
            new Error(
                ErrorCode.OperationFailed,
                "Test exit failure."));

        Result firstResult = _stateMachine.ChangeState(firstState);

        Assert.That(firstResult.IsSuccess, Is.True);

        Result result = _stateMachine.ChangeState(secondState);

        Assert.Multiple(() =>
        {
            Assert.That(
                result.IsSuccess,
                Is.False,
                "The transition must fail when the previous state cannot exit.");

            Assert.That(
                result.Error,
                Is.Not.Null);

            Assert.That(
                result.Error!.Code,
                Is.EqualTo(ErrorCode.StateExitFailed));

            Assert.That(
                _stateMachine.CurrentState,
                Is.SameAs(firstState),
                "The previous state must remain current when Exit fails.");

            Assert.That(
                secondState.EnterCount,
                Is.EqualTo(0),
                "The new state must not be entered.");

            Assert.That(
                firstState.ExitCount,
                Is.EqualTo(1),
                "Exit must still have been attempted exactly once.");
        });
    }

    // ------------------------------------------------------------------------
    // Enter failure
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_WhenEnterFails_ShouldRejectTransition()
    {
        TestState firstState = new TestState("StateA");
        TestState secondState = new TestState("StateB");

        secondState.EnterResult = Result.Failure(
            new Error(
                ErrorCode.OperationFailed,
                "Test enter failure."));

        Result firstResult = _stateMachine.ChangeState(firstState);

        Assert.That(firstResult.IsSuccess, Is.True);

        Result result = _stateMachine.ChangeState(secondState);

        Assert.Multiple(() =>
        {
            Assert.That(
                result.IsSuccess,
                Is.False,
                "The transition must fail when the new state cannot enter.");

            Assert.That(
                result.Error,
                Is.Not.Null);

            Assert.That(
                result.Error!.Code,
                Is.EqualTo(ErrorCode.StateEnterFailed));

            Assert.That(
                secondState.EnterCount,
                Is.EqualTo(1),
                "Enter must have been attempted.");

            Assert.That(
                firstState.ExitCount,
                Is.EqualTo(1),
                "The previous state must have been exited.");

            Assert.That(
                _stateMachine.CurrentState,
                Is.SameAs(firstState),
                "The state machine should restore the previous state.");
        });
    }

    // ------------------------------------------------------------------------
    // Enter failure and restoration
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_WhenEnterFails_ShouldReEnterPreviousState()
    {
        TestState firstState = new TestState("StateA");
        TestState secondState = new TestState("StateB");

        secondState.EnterResult = Result.Failure(
            new Error(
                ErrorCode.OperationFailed,
                "Test enter failure."));

        Result firstResult = _stateMachine.ChangeState(firstState);

        Assert.That(firstResult.IsSuccess, Is.True);

        Result result = _stateMachine.ChangeState(secondState);

        Assert.That(result.IsSuccess, Is.False);

        Assert.That(
            firstState.EnterCount,
            Is.EqualTo(2),
            "The previous state must be re-entered after the new state fails.");

        Assert.That(
            _stateMachine.CurrentState,
            Is.SameAs(firstState));
    }

    // ------------------------------------------------------------------------
    // Null runtime argument
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_WhenNullIsProvided_ShouldReturnFailure()
    {
        TestState initialState = new TestState("StateA");

        Result initialResult = _stateMachine.ChangeState(initialState);

        Assert.That(initialResult.IsSuccess, Is.True);

        Result result = _stateMachine.ChangeState(null!);

        Assert.Multiple(() =>
        {
            Assert.That(
                result.IsSuccess,
                Is.False,
                "A null state must be rejected.");

            Assert.That(
                result.Error,
                Is.Not.Null);

            Assert.That(
                result.Error!.Code,
                Is.EqualTo(ErrorCode.InvalidArgument));

            Assert.That(
                _stateMachine.CurrentState,
                Is.SameAs(initialState),
                "Invalid input must not modify the current state.");

            Assert.That(
                initialState.ExitCount,
                Is.EqualTo(0),
                "Invalid input must not cause the current state to exit.");
        });
    }

    // ------------------------------------------------------------------------
    // Unexpected exception during Exit
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_WhenExitThrows_ShouldReturnFailure()
    {
        TestState firstState = new TestState("StateA");
        TestState secondState = new TestState("StateB");

        Exception expectedException =
            new InvalidOperationException(
                "Test exception from Exit.");

        firstState.ExitException = expectedException;

        Result firstResult = _stateMachine.ChangeState(firstState);

        Assert.That(firstResult.IsSuccess, Is.True);

        Result result = _stateMachine.ChangeState(secondState);

        Assert.Multiple(() =>
        {
            Assert.That(
                result.IsSuccess,
                Is.False,
                "An unexpected Exit exception must result in failure.");

            Assert.That(
                result.Error,
                Is.Not.Null);

            Assert.That(
                result.Error!.Code,
                Is.EqualTo(ErrorCode.StateExitFailed));

            Assert.That(
                result.Error.Exception,
                Is.SameAs(expectedException));

            Assert.That(
                _stateMachine.CurrentState,
                Is.SameAs(firstState));

            Assert.That(
                secondState.EnterCount,
                Is.EqualTo(0));
        });
    }

    // ------------------------------------------------------------------------
    // Unexpected exception during Enter
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_WhenEnterThrows_ShouldReturnFailure()
    {
        TestState firstState = new TestState("StateA");
        TestState secondState = new TestState("StateB");

        Exception expectedException =
            new InvalidOperationException(
                "Test exception from Enter.");

        secondState.EnterException = expectedException;

        Result firstResult = _stateMachine.ChangeState(firstState);

        Assert.That(firstResult.IsSuccess, Is.True);

        Result result = _stateMachine.ChangeState(secondState);

        Assert.Multiple(() =>
        {
            Assert.That(
                result.IsSuccess,
                Is.False,
                "An unexpected Enter exception must result in failure.");

            Assert.That(
                result.Error,
                Is.Not.Null);

            Assert.That(
                result.Error!.Code,
                Is.EqualTo(ErrorCode.StateEnterFailed));

            Assert.That(
                result.Error.Exception,
                Is.SameAs(expectedException));

            Assert.That(
                _stateMachine.CurrentState,
                Is.SameAs(firstState),
                "The previous state should be restored.");

            Assert.That(
                firstState.EnterCount,
                Is.EqualTo(2));
        });
    }

    // ------------------------------------------------------------------------
    // Logging
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_SuccessfulTransition_ShouldWriteDebugLog()
    {
        TestState state = new TestState("StateA");

        Result result = _stateMachine.ChangeState(state);

        Assert.That(result.IsSuccess, Is.True);

        Assert.That(
            _logger.ContainsLevel(LogLevel.Debug),
            Is.True,
            "A successful state transition should produce a Debug log.");
    }

    [Test]
    public void ChangeState_SameState_ShouldWriteDebugLog()
    {
        TestState state = new TestState("StateA");

        Result firstResult = _stateMachine.ChangeState(state);

        Assert.That(firstResult.IsSuccess, Is.True);

        _logger.Clear();

        Result secondResult = _stateMachine.ChangeState(state);

        Assert.That(secondResult.IsSuccess, Is.True);

        Assert.That(
            _logger.ContainsLevel(LogLevel.Debug),
            Is.True,
            "Ignoring a same-state transition should produce a Debug log.");
    }

    [Test]
    public void ChangeState_WhenExitFails_ShouldWriteErrorLog()
    {
        TestState firstState = new TestState("StateA");
        TestState secondState = new TestState("StateB");

        firstState.ExitResult = Result.Failure(
            new Error(
                ErrorCode.OperationFailed,
                "Test failure."));

        Result firstResult = _stateMachine.ChangeState(firstState);

        Assert.That(firstResult.IsSuccess, Is.True);

        _logger.Clear();

        Result result = _stateMachine.ChangeState(secondState);

        Assert.That(result.IsSuccess, Is.False);

        Assert.That(
            _logger.ContainsLevel(LogLevel.Error),
            Is.True,
            "An Exit failure should produce an Error log.");
    }

    [Test]
    public void ChangeState_WhenEnterFails_ShouldWriteErrorLog()
    {
        TestState firstState = new TestState("StateA");
        TestState secondState = new TestState("StateB");

        secondState.EnterResult = Result.Failure(
            new Error(
                ErrorCode.OperationFailed,
                "Test failure."));

        Result firstResult = _stateMachine.ChangeState(firstState);

        Assert.That(firstResult.IsSuccess, Is.True);

        _logger.Clear();

        Result result = _stateMachine.ChangeState(secondState);

        Assert.That(result.IsSuccess, Is.False);

        Assert.That(
            _logger.ContainsLevel(LogLevel.Error),
            Is.True,
            "An Enter failure should produce an Error log.");
    }

    [Test]
    public void ChangeState_WhenNullIsProvided_ShouldWriteErrorLog()
    {
        Result result = _stateMachine.ChangeState(null!);

        Assert.That(result.IsSuccess, Is.False);

        Assert.That(
            _logger.ContainsLevel(LogLevel.Error),
            Is.True,
            "Invalid input should produce an Error log.");
    }

    // ------------------------------------------------------------------------
    // Multiple transitions
    // ------------------------------------------------------------------------

    [Test]
    public void ChangeState_MultipleTransitions_ShouldMaintainCorrectCurrentState()
    {
        TestState stateA = new TestState("StateA");
        TestState stateB = new TestState("StateB");
        TestState stateC = new TestState("StateC");

        Result resultA = _stateMachine.ChangeState(stateA);
        Result resultB = _stateMachine.ChangeState(stateB);
        Result resultC = _stateMachine.ChangeState(stateC);

        Assert.Multiple(() =>
        {
            Assert.That(resultA.IsSuccess, Is.True);
            Assert.That(resultB.IsSuccess, Is.True);
            Assert.That(resultC.IsSuccess, Is.True);

            Assert.That(
                _stateMachine.CurrentState,
                Is.SameAs(stateC));

            Assert.That(
                stateA.EnterCount,
                Is.EqualTo(1));

            Assert.That(
                stateA.ExitCount,
                Is.EqualTo(1));

            Assert.That(
                stateB.EnterCount,
                Is.EqualTo(1));

            Assert.That(
                stateB.ExitCount,
                Is.EqualTo(1));

            Assert.That(
                stateC.EnterCount,
                Is.EqualTo(1));

            Assert.That(
                stateC.ExitCount,
                Is.EqualTo(0));
        });
    }

    // ------------------------------------------------------------------------
    // Test helpers
    // ------------------------------------------------------------------------

    private sealed class TestState : IState
    {
        public string Name { get; }

        public int EnterCount { get; private set; }

        public int ExitCount { get; private set; }

        public Result EnterResult { get; set; } =
            Result.Success();

        public Result ExitResult { get; set; } =
            Result.Success();

        public Exception? EnterException { get; set; }

        public Exception? ExitException { get; set; }

        public Action? OnEnter { get; set; }

        public Action? OnExit { get; set; }

        public TestState(string name)
        {
            Name = name;
        }

        public Result Enter()
        {
            EnterCount++;

            OnEnter?.Invoke();

            if (EnterException is not null)
            {
                throw EnterException;
            }

            return EnterResult;
        }

        public Result Exit()
        {
            ExitCount++;

            OnExit?.Invoke();

            if (ExitException is not null)
            {
                throw ExitException;
            }

            return ExitResult;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    private sealed class TestLogger : ILogger
    {
        public List<LogEntry> Entries { get; } = new();

        public void Log(
            LogLevel level,
            string message)
        {
            Entries.Add(
                new LogEntry(
                    level,
                    message,
                    null));
        }

        public void Log(
            LogLevel level,
            string message,
            Exception exception)
        {
            Entries.Add(
                new LogEntry(
                    level,
                    message,
                    exception));
        }

        public bool ContainsLevel(LogLevel level)
        {
            return Entries.Any(
                entry => entry.Level == level);
        }

        public void Clear()
        {
            Entries.Clear();
        }
    }

    private sealed record LogEntry(
        LogLevel Level,
        string Message,
        Exception? Exception);
}