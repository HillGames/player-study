using NUnit.Framework;
using PlayerStudy.Core.Common.Math;
using PlayerStudy.Core.Player.Movement;

namespace PlayerStudy.Test.Player.Movement;

[TestFixture]
public sealed class PlayerMovementTest
{
    private static PlayerMovementSettings CreateSettings()
    {
        return new PlayerMovementSettings(
            maxSpeed: 100.0f,
            acceleration: 50.0f,
            deceleration: 100.0f);
    }

    [Test]
    public void StartsWithZeroVelocity()
    {
        var movement = new PlayerMovement(CreateSettings());

        Assert.That(
            movement.Velocity,
            Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void FullInputAcceleratesTowardMaximumSpeed()
    {
        var movement = new PlayerMovement(CreateSettings());

        movement.Update(
            new Vector2(1.0f, 0.0f),
            1.0f);

        Assert.That(
            movement.Velocity,
            Is.EqualTo(new Vector2(50.0f, 0.0f)));
    }

    [Test]
    public void FullInputReachesMaximumSpeedAfterEnoughTime()
    {
        var movement = new PlayerMovement(CreateSettings());

        movement.Update(
            new Vector2(1.0f, 0.0f),
            1.0f);

        movement.Update(
            new Vector2(1.0f, 0.0f),
            1.0f);

        Assert.That(
            movement.Velocity,
            Is.EqualTo(new Vector2(100.0f, 0.0f)));
    }

    [Test]
    public void PartialInputProducesPartialTargetSpeed()
    {
        var movement = new PlayerMovement(CreateSettings());

        movement.Update(
            new Vector2(0.5f, 0.0f),
            2.0f);

        Assert.That(
            movement.Velocity,
            Is.EqualTo(new Vector2(50.0f, 0.0f)));
    }

    [Test]
    public void ReleasingInputDeceleratesTowardZero()
    {
        var movement = new PlayerMovement(CreateSettings());

        movement.Update(
            new Vector2(1.0f, 0.0f),
            2.0f);

        movement.Update(
            Vector2.Zero,
            0.5f);

        Assert.That(
            movement.Velocity,
            Is.EqualTo(new Vector2(50.0f, 0.0f)));
    }

    [Test]
    public void ReleasingInputEventuallyStopsPlayer()
    {
        var movement = new PlayerMovement(CreateSettings());

        movement.Update(
            new Vector2(1.0f, 0.0f),
            2.0f);

        movement.Update(
            Vector2.Zero,
            1.0f);

        Assert.That(
            movement.Velocity,
            Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void NegativeInputProducesNegativeVelocity()
    {
        var movement = new PlayerMovement(CreateSettings());

        movement.Update(
            new Vector2(-1.0f, 0.0f),
            10.0f);

        Assert.That(
            movement.Velocity,
            Is.EqualTo(new Vector2(-100.0f, 0.0f)));
    }

    [Test]
    public void VerticalInputProducesVerticalVelocity()
    {
        var movement = new PlayerMovement(CreateSettings());

        movement.Update(
            new Vector2(0.0f, -1.0f),
            10.0f);

        Assert.That(
            movement.Velocity,
            Is.EqualTo(new Vector2(0.0f, -100.0f)));
    }

    [Test]
    public void DiagonalInputDoesNotExceedMaximumSpeed()
    {
        var movement = new PlayerMovement(CreateSettings());

        movement.Update(
            new Vector2(1.0f, 1.0f),
            10.0f);

        Assert.That(
            movement.Velocity.Length,
            Is.EqualTo(100.0f).Within(0.001f));
    }

    [Test]
    public void InputAboveMaximumMagnitudeIsClamped()
    {
        var movement = new PlayerMovement(CreateSettings());

        movement.Update(
            new Vector2(2.0f, 0.0f),
            10.0f);

        Assert.That(
            movement.Velocity,
            Is.EqualTo(new Vector2(100.0f, 0.0f)));
    }

    [Test]
    public void PartialAnalogInputMaintainsItsMagnitude()
    {
        var movement = new PlayerMovement(CreateSettings());

        movement.Update(
            new Vector2(0.25f, 0.0f),
            10.0f);

        Assert.That(
            movement.Velocity,
            Is.EqualTo(new Vector2(25.0f, 0.0f)));
    }

    [Test]
    public void DirectionIsPreservedWithPartialInput()
    {
        var movement = new PlayerMovement(CreateSettings());

        var input = new Vector2(0.0f, -0.5f);

        movement.Update(input, 10.0f);

        Assert.That(
            movement.Velocity,
            Is.EqualTo(new Vector2(0.0f, -50.0f)));
    }

    [Test]
    public void NegativeDeltaTimeIsRejected()
    {
        var movement = new PlayerMovement(CreateSettings());

        Assert.That(
            () => movement.Update(
                new Vector2(1.0f, 0.0f),
                -1.0f),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }
}