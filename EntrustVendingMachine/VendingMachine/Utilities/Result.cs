using VendingMachine.Enums;

namespace EntrustVendingMachine.Utilities
{
    /// <summary>
    /// Represents the result of an operation, encapsulating success, failure, error messages, and an optional return value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the operation.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// The error message associated with a failed operation. Null if the operation was successful.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// The error code identifying the type of failure. Defaults to <see cref="ErrorCode.None"/> for successful operations.
        /// </summary>
        public ErrorCode ErrorCode { get; }

        /// <summary>
        /// The value returned by the operation. May be the default value of <typeparamref name="T"/> in case of failure.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="isSuccess">Whether the operation was successful.</param>
        /// <param name="value">The value returned by the operation.</param>
        /// <param name="error">The error message, if the operation failed.</param>
        /// <param name="errorCode">The error code identifying the type of failure.</param>
        private Result(bool isSuccess, T value, string error, ErrorCode errorCode)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Creates a successful result with the specified value.
        /// </summary>
        /// <param name="value">The value returned by the operation.</param>
        /// <returns>A <see cref="Result{T}"/> indicating success.</returns>
        public static Result<T> Success(T value) => new Result<T>(true, value, null, ErrorCode.None);

        /// <summary>
        /// Creates a failed result with the specified error message and error code.
        /// </summary>
        /// <param name="error">The error message describing the failure.</param>
        /// <param name="errorCode">The error code identifying the type of failure. Defaults to <see cref="ErrorCode.MachineError"/>.</param>
        /// <returns>A <see cref="Result{T}"/> indicating failure.</returns>
        public static Result<T> Fail(string error, ErrorCode errorCode = ErrorCode.MachineError)
            => new Result<T>(false, default, error, errorCode);

        /// <summary>
        /// Creates a failed result with the specified error message, value, and error code.
        /// </summary>
        /// <param name="error">The error message describing the failure.</param>
        /// <param name="value">The value to return, even if the operation failed.</param>
        /// <param name="errorCode">The error code identifying the type of failure. Defaults to <see cref="ErrorCode.MachineError"/>.</param>
        /// <returns>A <see cref="Result{T}"/> indicating failure, with an associated value.</returns>
        public static Result<T> FailWithValue(string error, T value = default, ErrorCode errorCode = ErrorCode.MachineError)
            => new Result<T>(false, value, error, errorCode);

        /// <summary>
        /// Indicates whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;
    }
}
