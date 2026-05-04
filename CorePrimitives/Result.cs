namespace CorePrimitives
{
    public enum ErrorType
    {
        NotFound,
        Conflict,
        Unauthorized,
    }

    public record Error(ErrorType Type, string Description);

    public record Result
    {
        public bool Succeeded => Error is null;//Errors.Count == 0;
        public bool Failed => Error is not null;//Errors.Count != 0;
        
        
        public Error? Error { get;  }//public List<Error> Errors { get; }

        //protected Result(List<Error> errors) => Errors = errors;
        protected Result(Error error) => Error = error;//Errors = [error];
        protected Result() => Error = null; // : this([]) { }

        //public static Result Fail(List<Error> errors) => new(errors);
        public static Result Fail(Error error) => new(error);
        public static Result Success() => new();

        //public static implicit operator Result(List<Error> errors) => new(errors);
        public static implicit operator Result(Error error) => new(error);
    }

    public record Result<T> : Result
    {
        private readonly T? _value;
        public T Value => Succeeded ? _value! : throw new InvalidOperationException("No value for failed result.");

        //private Result(List<Error> errors) : base(errors) { }
        private Result(Error error) : base(error) { }
        private Result(T value) : base() => _value = value;

        //public static implicit operator Result<T>(List<Error> errors) => new(errors);
        public static implicit operator Result<T>(Error error) => new(error);
        public static implicit operator Result<T>(T value) => new(value);
    }

    public static class  ResultExtensions
    {
        public static Result Then(this Result result, Func<Result> next) => 
            result.Failed ? result : next();

        public static Result<T> Then<T>(this Result result, Func<Result<T>> next) =>
            result.Failed ? result.Error! : next();//result.Failed ? result.Errors : next();
    }        
}
