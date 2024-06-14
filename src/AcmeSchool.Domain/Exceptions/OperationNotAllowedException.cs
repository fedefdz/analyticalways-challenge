namespace AcmeSchool.Domain.Exceptions
{
    public class OperationNotAllowedException : DomainException
    {
        public OperationNotAllowedException(string message) 
            : base((int)DomainErrorCodes.OperationNotAllowed, message)
        {
        }
    }
}
