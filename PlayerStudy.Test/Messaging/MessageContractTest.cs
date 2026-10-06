using NUnit.Framework;
using PlayerStudy.Core.Messaging;

namespace PlayerStudy.Test.Messaging;

[TestFixture]
public class MessageContractTest
{
    [Test]
    public void Command_ShouldBeMessage()
    {
        IMessage message = new TestCommand();

        Assert.That(message, Is.Not.Null);
        Assert.That(message, Is.TypeOf<TestCommand>());
    }

    [Test]
    public void Event_ShouldBeMessage()
    {
        IMessage message = new TestEvent();

        Assert.That(message, Is.Not.Null);
        Assert.That(message, Is.TypeOf<TestEvent>());
    }

    [Test]
    public void Command_ShouldImplementICommand()
    {
        var command = new TestCommand();

        Assert.That(command, Is.InstanceOf<ICommand>());
        Assert.That(command, Is.InstanceOf<IMessage>());
    }

    [Test]
    public void Event_ShouldImplementIEvent()
    {
        var @event = new TestEvent();

        Assert.That(@event, Is.InstanceOf<IEvent>());
        Assert.That(@event, Is.InstanceOf<IMessage>());
    }

    [Test]
    public void Command_ShouldBeDistinctFromEvent()
    {
        var command = new TestCommand();

        Assert.That(command, Is.InstanceOf<ICommand>());
        Assert.That(command, Is.Not.InstanceOf<IEvent>());
    }

    [Test]
    public void Event_ShouldBeDistinctFromCommand()
    {
        var @event = new TestEvent();

        Assert.That(@event, Is.InstanceOf<IEvent>());
        Assert.That(@event, Is.Not.InstanceOf<ICommand>());
    }

    [Test]
    public void ConcreteCommand_ShouldBeAbleToCarryCommandData()
    {
        var command = new TestCommand(42);

        Assert.That(command.Value, Is.EqualTo(42));
    }

    [Test]
    public void ConcreteEvent_ShouldBeAbleToCarryEventData()
    {
        var @event = new TestEvent(100);

        Assert.That(@event.Value, Is.EqualTo(100));
    }

    private sealed class TestCommand : ICommand
    {
        public int Value { get; }

        public TestCommand(int value = 0)
        {
            Value = value;
        }
    }

    private sealed class TestEvent : IEvent
    {
        public int Value { get; }

        public TestEvent(int value = 0)
        {
            Value = value;
        }
    }
}