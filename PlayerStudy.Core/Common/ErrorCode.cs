namespace PlayerStudy.Core.Common;

public enum ErrorCode
{
	None = 0,
	InvalidArgument,
	InvalidState,
	StateTransitionFailed,
	StateEnterFailed,
	StateExitFailed,
	AlreadyInitialized,
	NotInitialized,
	OperationFailed,
	UnexpectedError
}