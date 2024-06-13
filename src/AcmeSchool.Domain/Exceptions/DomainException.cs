namespace AcmeSchool.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        public int ErrorCode { get; }

        protected DomainException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
