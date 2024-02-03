namespace GuessWordClient.Exceptions;

public sealed class ErrorResultException : Exception {
	public readonly ErrorCode ErrorCode;

	public ErrorResultException(ErrorResult errorResult) : base(errorResult.Detail) {
		ErrorCode = errorResult.ErrorCode;
		Data["ErrorCode"] = ErrorCode;
	}
}
