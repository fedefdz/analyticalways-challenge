namespace AcmeSchool.Application.Services.PaymentGateway
{
    public static class PaymentResultCodes
    {
        public const string Success = "SUCCESS_PAYMENT";
        public const string InvalidCard = "INVALID_CARD";
        public const string InsufficientFunds = "INSUFFICIENT_FUNDS";
        public const string PaymentGatewayError = "PAYMENT_GATEWAY_ERROR";
        public const string InvalidPaymentData = "INVALID_PAYMENT_DATA";
        public const string UnknownError = "UNKNOWN_ERROR";
    }
}
