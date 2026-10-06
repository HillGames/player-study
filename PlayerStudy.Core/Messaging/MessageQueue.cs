using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using PlayerStudy.Core.Common;

namespace PlayerStudy.Core.Messaging;

public sealed class MessageQueue
{
	private readonly Queue<IMessage> _messages = new Queue<IMessage>();
	public int Count => _messages.Count;
	public bool IsEmpty =>_messages.Count == 0;
	public Result Enqueue(IMessage msg)
	{
		if (msg is null)
		{
			return Result.Failure(new Error(ErrorCode.InvalidArgument,
					"Message Cannot be null."));
		}
		_messages.Enqueue(msg);
		return Result.Success();
	}

	public bool TryDequeue(out IMessage? msg)
	{
		if(_messages.Count == 0)
		{
			msg = null;
			return false;
		}
		msg = _messages.Dequeue();
		return true;
	}

}