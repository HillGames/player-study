using System.Collections.Generic;

namespace Game.Core.Messaging;

public sealed class MessageQueue
{
	private readonly Queue<IMessage> _queue = new();
	public int Count => _queue.Count;
	public bool isEmpty => _queue.Count == 0;
	public void Enqueue(IMessage message)
	{
		_queue.Enqueue(message);
	}
	public IMessage Dequeue()
	{
		return _queue.Dequeue();
	
	}

	public void Clear()
	{
		_queue.Clear();
	}
}