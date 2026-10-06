using NUnit.Framework;
using PlayerStudy.Core.Messaging;

namespace PlayerStudy.Test.Messaging;

[TestFixture]
public class MessageQueueTest
{
    [Test]
    public void NewQueue_ShouldBeEmpty()
    {
        var queue = new MessageQueue();

        Assert.That(queue.IsEmpty, Is.True);
        Assert.That(queue.Count, Is.EqualTo(0));
    }

    [Test]
    public void Enqueue_ValidMessage_ShouldSucceed()
    {
        var queue = new MessageQueue();
        var message = new TestMessage();

        var result = queue.Enqueue(message);

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void Enqueue_ValidMessage_ShouldIncreaseCount()
    {
        var queue = new MessageQueue();

        queue.Enqueue(new TestMessage());

        Assert.That(queue.Count, Is.EqualTo(1));
    }

    [Test]
    public void Enqueue_ValidMessage_ShouldMakeQueueNonEmpty()
    {
        var queue = new MessageQueue();

        queue.Enqueue(new TestMessage());

        Assert.That(queue.IsEmpty, Is.False);
    }

    [Test]
    public void Enqueue_MultipleMessages_ShouldIncreaseCountCorrectly()
    {
        var queue = new MessageQueue();

        queue.Enqueue(new TestMessage());
        queue.Enqueue(new TestMessage());
        queue.Enqueue(new TestMessage());

        Assert.That(queue.Count, Is.EqualTo(3));
    }

    [Test]
    public void Enqueue_NullMessage_ShouldFail()
    {
        var queue = new MessageQueue();

        var result = queue.Enqueue(null!);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Enqueue_NullMessage_ShouldNotChangeQueue()
    {
        var queue = new MessageQueue();

        queue.Enqueue(null!);

        Assert.That(queue.Count, Is.EqualTo(0));
        Assert.That(queue.IsEmpty, Is.True);
    }

    [Test]
    public void TryDequeue_ShouldReturnOldestMessage()
    {
        var queue = new MessageQueue();
        var first = new TestMessage();
        var second = new TestMessage();

        queue.Enqueue(first);
        queue.Enqueue(second);

        var success = queue.TryDequeue(out var message);

        Assert.That(success, Is.True);
        Assert.That(message, Is.SameAs(first));
    }

    [Test]
    public void TryDequeue_ShouldRemoveMessage()
    {
        var queue = new MessageQueue();

        queue.Enqueue(new TestMessage());

        queue.TryDequeue(out _);

        Assert.That(queue.Count, Is.EqualTo(0));
        Assert.That(queue.IsEmpty, Is.True);
    }

    [Test]
    public void TryDequeue_ShouldDecreaseCount()
    {
        var queue = new MessageQueue();

        queue.Enqueue(new TestMessage());
        queue.Enqueue(new TestMessage());

        queue.TryDequeue(out _);

        Assert.That(queue.Count, Is.EqualTo(1));
    }

    [Test]
    public void TryDequeue_EmptyQueue_ShouldReturnFalse()
    {
        var queue = new MessageQueue();

        var success = queue.TryDequeue(out _);

        Assert.That(success, Is.False);
    }

    [Test]
    public void TryDequeue_EmptyQueue_ShouldReturnNullMessage()
    {
        var queue = new MessageQueue();

        var success = queue.TryDequeue(out var message);

        Assert.That(success, Is.False);
        Assert.That(message, Is.Null);
    }

    [Test]
    public void TryDequeue_EmptyQueue_ShouldNotChangeQueueState()
    {
        var queue = new MessageQueue();

        queue.TryDequeue(out _);

        Assert.That(queue.Count, Is.EqualTo(0));
        Assert.That(queue.IsEmpty, Is.True);
    }

    [Test]
    public void TryDequeue_ShouldPreserveFifoOrder()
    {
        var queue = new MessageQueue();

        var first = new TestMessage();
        var second = new TestMessage();
        var third = new TestMessage();

        queue.Enqueue(first);
        queue.Enqueue(second);
        queue.Enqueue(third);

        queue.TryDequeue(out var firstResult);
        queue.TryDequeue(out var secondResult);
        queue.TryDequeue(out var thirdResult);

        Assert.That(firstResult, Is.SameAs(first));
        Assert.That(secondResult, Is.SameAs(second));
        Assert.That(thirdResult, Is.SameAs(third));
    }

    [Test]
    public void Queue_ShouldAcceptCommand()
    {
        var queue = new MessageQueue();

        var result = queue.Enqueue(new TestCommand());

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(queue.Count, Is.EqualTo(1));
    }

    [Test]
    public void Queue_ShouldAcceptEvent()
    {
        var queue = new MessageQueue();

        var result = queue.Enqueue(new TestEvent());

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(queue.Count, Is.EqualTo(1));
    }

    [Test]
    public void Queue_ShouldPreserveConcreteMessageType()
    {
        var queue = new MessageQueue();
        var command = new TestCommand();

        queue.Enqueue(command);

        queue.TryDequeue(out var message);

        Assert.That(message, Is.SameAs(command));
        Assert.That(message, Is.TypeOf<TestCommand>());
    }

    [Test]
    public void Queue_ShouldMaintainCorrectStateThroughMultipleOperations()
    {
        var queue = new MessageQueue();

        var first = new TestMessage();
        var second = new TestMessage();

        queue.Enqueue(first);
        queue.Enqueue(second);

        Assert.That(queue.Count, Is.EqualTo(2));
        Assert.That(queue.IsEmpty, Is.False);

        queue.TryDequeue(out _);

        Assert.That(queue.Count, Is.EqualTo(1));
        Assert.That(queue.IsEmpty, Is.False);

        queue.TryDequeue(out _);

        Assert.That(queue.Count, Is.EqualTo(0));
        Assert.That(queue.IsEmpty, Is.True);

        var success = queue.TryDequeue(out var message);

        Assert.That(success, Is.False);
        Assert.That(message, Is.Null);
    }

    private sealed class TestMessage : IMessage
    {
    }

    private sealed class TestCommand : ICommand
    {
    }

    private sealed class TestEvent : IEvent
    {
    }
}