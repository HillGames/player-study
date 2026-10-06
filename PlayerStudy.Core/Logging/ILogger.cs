namespace PlayerStudy.Core.Logging;

public interface ILogger
{
	void Log(LogLevel level, 
			string message);

	void Log(LogLevel level,
			string message,
			Exception exception);
}