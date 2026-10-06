using NUnit.Framework;
using PlayerStudy.Core.Common;
using PlayerStudy.Core.Player.States;
using PlayerStudy.Core.StateMachine;

namespace PlayerStudy.Test.Player.States;

[TestFixture]
public sealed class PlayerStateTest
{
    [Test]
    public void IdleStateImplementsIState()
    {
        var state = new PlayerIdleState();

        Assert.That(state, Is.InstanceOf<IState>());
    }

    [Test]
    public void MovingStateImplementsIState()
    {
        var state = new PlayerMovingState();

        Assert.That(state, Is.InstanceOf<IState>());
    }

    [Test]
    public void IdleStateEnterSucceeds()
    {
        var state = new PlayerIdleState();

        var result = state.Enter();

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void IdleStateExitSucceeds()
    {
        var state = new PlayerIdleState();

        var result = state.Exit();

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void MovingStateEnterSucceeds()
    {
        var state = new PlayerMovingState();

        var result = state.Enter();

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void MovingStateExitSucceeds()
    {
        var state = new PlayerMovingState();

        var result = state.Exit();

        Assert.That(result.IsSuccess, Is.True);
    }
}