namespace AcmeSchool.Application.Services.PaymentGateway.DTO
{
    public record CreditCardPaymentRequest : PaymentRequest
    {
        public required string CardNumber { get; init; }
        public required string CardHolder { get; init; }
        public required string ExpiryDate { get; init; }
        public required string CVV { get; init; }
        public required int Installments { get; init; }
    }
}
