namespace PlayerStudy.Core.Common;

public sealed class Error
{
	public ErrorCode Code {get;}
	public string Message {get;}
	public Exception? Exception{get;}
	public Error(ErrorCode code, 
			string message, 
			Exception? exception = null)
	{
		Code = code;
		Message = message;
		Exception = exception;
	}

	public override string ToString()
	{
		return $"{Code}: {Message}";
	}

}