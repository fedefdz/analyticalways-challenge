namespace AcmeSchool.Domain.Exceptions
{
    public class PaymentInvalidDataException : DomainException
    {
        public PaymentInvalidDataException(string fieldName, string message) :
            base((int)DomainErrorCodes.PaymentInvalidData, $"payment '{fieldName}' {message}")
        { }
    }
}
