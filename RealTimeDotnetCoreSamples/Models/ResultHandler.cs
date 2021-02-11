using System;
using System.Diagnostics.Contracts;

namespace RealTimeDotnetCoreSamples.Models
{
    public class ResultHandler
    {
        public bool Success { get; private set; }
        public bool IsFinished { get; private set; }
        public E_ErrorType ErrorType { get; private set; }
        public Exception Exception { get; private set; }
        public string Message { get; private set; }

        public ResultHandler(bool success, E_ErrorType errorType, string customErrorMessage)
        {
            Contract.Requires(success);

            this.Success = success;
            this.ErrorType = errorType;
            this.Message = customErrorMessage;
        }

        public ResultHandler(Exception error)
        {
            Contract.Requires(error != null);

            this.Success = false;
            this.Exception = error;
            this.ErrorType = E_ErrorType.Exception;
        }

        public static ResultHandler<T> Fail<T>(E_ErrorType errorType)
        {
            return new ResultHandler<T>(default, false, errorType, errorType.ToString());
        }

        public static ResultHandler<T> Fail<T>(E_ErrorType errorType, string customErrorMessage)
        {
            return new ResultHandler<T>(default, false, errorType, customErrorMessage);
        }

        public static ResultHandler<T> Fail<T>(Exception exception)
        {
            return new ResultHandler<T>(exception);
        }

        public static ResultHandler<T> Ok<T>(T value)
        {
            if (value == null)
                return new ResultHandler<T>(value, false, E_ErrorType.DataNotFound, null);
            else
                return new ResultHandler<T>(value, true, E_ErrorType.Undefined, null);
        }

        public static ResultHandler<T> Finish<T>(T value)
        {
            return new ResultHandler<T>(value, true, E_ErrorType.Undefined, null);
        }
    }

    public class ResultHandler<T> : ResultHandler
    {
        private T _value;

        public T Value
        {
            get
            {
                Contract.Requires(Success);

                return _value;
            }
            private set { _value = value; }
        }

        public ResultHandler(T value, bool success, E_ErrorType ErrorType, string message)
            : base(success, ErrorType, message)
        {
            Contract.Requires(value != null || !success);

            this.Value = value;
        }

        public ResultHandler(Exception exception)
            : base(exception)
        {
        }
    }

    public enum E_ErrorType
    {
        Undefined = 0,
        Unknown = 1,
        BadRequest = 2,
        DataNotFound = 3,
        Exception = 4,
        Supplier = 6,
        EntityNotValid = 7
    }
}