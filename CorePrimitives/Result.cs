namespace CorePrimitives
{
    public record Failure(string Id, string Description);

    public record Result
    {
        public bool IsSuccess => FailureValue is null;
        public bool IsFailure => FailureValue is not null;
        public Failure? FailureValue { get; }
        public Failure Failure => FailureValue ?? throw new InvalidOperationException("Check IsFailure before accessing Failure.");
        protected Result(Failure? failureValue) => FailureValue = failureValue;
        public static Result Success() => new(null);
        public static Result Fail(Failure failureValue) => new(failureValue);
        public static implicit operator Result(Failure failureValue) => new(failureValue);
    }

    public record Result<T> : Result
    {
        private readonly T? _value;
        public T Value => IsSuccess ? _value! : throw new InvalidOperationException("No value for failed result.");
        private Result(T value) : base(null) => _value = value;
        private Result(Failure failureValue) : base(failureValue) { }
        public static implicit operator Result<T>(T value) => new(value);
        public static implicit operator Result<T>(Failure failureValue) => new(failureValue);
    }

    public static class  ResultExtenstions
    {
        public static Result Then(this Result result, Func<Result> next) => 
            result.IsFailure ? result : next();

        public static Result<T> Then<T>(this Result result, Func<Result<T>> next) => 
            result.IsFailure ? result.Failure : next();
    }        
}
