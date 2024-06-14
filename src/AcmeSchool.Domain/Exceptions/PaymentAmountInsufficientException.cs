namespace AcmeSchool.Domain.Exceptions
{
    public class PaymentAmountInsufficientException : DomainException
    {
        public PaymentAmountInsufficientException() :
            base((int)DomainErrorCodes.PaymentAmountInsufficient, "payment amount is insufficient")
        { }
    }
}
