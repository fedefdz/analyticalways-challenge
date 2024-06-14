namespace AcmeSchool.Application.Services.PaymentGateway.DTO
{
    public record DebitCardPaymentRequest : PaymentRequest
    {
        public required string CardNumber { get; init; }
        public required string CardHolder { get; init; }
        public required DateTime ExpiryDate { get; init; }
        public required string CVV { get; init; }
    }
}
