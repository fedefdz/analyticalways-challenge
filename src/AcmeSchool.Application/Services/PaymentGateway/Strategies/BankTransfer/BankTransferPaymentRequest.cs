namespace AcmeSchool.Application.Services.PaymentGateway.Strategies.BankTransfer
{
    public record BankTransferPaymentRequest : PaymentRequest
    {
        public required string BankName { get; init; }
        public required string AccountNumber { get; init; }
        public required string AccountHolder { get; init; }
    }
}
