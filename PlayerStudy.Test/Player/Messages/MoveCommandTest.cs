using NUnit.Framework;
using PlayerStudy.Core.Common.Math;
using PlayerStudy.Core.Messaging;
using PlayerStudy.Core.Player.Messages;

namespace PlayerStudy.Test.Player.Messages;

[TestFixture]
public sealed class MoveCommandTest
{
    [Test]
    public void ImplementsICommand()
    {
        var command = new MoveCommand(Vector2.Zero);

        Assert.That(command, Is.InstanceOf<ICommand>());
    }

    [Test]
    public void ImplementsIMessage()
    {
        var command = new MoveCommand(Vector2.Zero);

        Assert.That(command, Is.InstanceOf<IMessage>());
    }

    [Test]
    public void StoresMovementInput()
    {
        var input = new Vector2(0.5f, -0.75f);

        var command = new MoveCommand(input);

        Assert.That(command.Input, Is.EqualTo(input));
    }

    [Test]
    public void SupportsZeroInput()
    {
        var command = new MoveCommand(Vector2.Zero);

        Assert.That(
            command.Input,
            Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void SupportsFullInput()
    {
        var input = new Vector2(1.0f, -1.0f);

        var command = new MoveCommand(input);

        Assert.That(
            command.Input,
            Is.EqualTo(input));
    }

    [Test]
    public void PreservesAnalogInputMagnitude()
    {
        var input = new Vector2(0.25f, 0.5f);

        var command = new MoveCommand(input);

        Assert.That(
            command.Input,
            Is.EqualTo(input));
    }

    [Test]
    public void CommandDoesNotModifyInput()
    {
        var input = new Vector2(2.0f, 0.5f);

        var command = new MoveCommand(input);

        Assert.That(
            command.Input,
            Is.EqualTo(input));
    }
}