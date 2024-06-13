namespace AcmeSchool.Application.Services.PaymentGateway.DTO
{
    public static class PaymentResultMessages
    {
        private static readonly Dictionary<string, string> Messages = new Dictionary<string, string>
    {
        { PaymentResultCodes.Success, "The payment was successful." },
        { PaymentResultCodes.InvalidCard, "The card number provided is invalid." },
        { PaymentResultCodes.InsufficientFunds, "There are insufficient funds in the account." },
        { PaymentResultCodes.PaymentGatewayError, "There was an error communicating with the payment gateway." },
        { PaymentResultCodes.InvalidPaymentData, "The payment data provided is invalid." },
        { PaymentResultCodes.UnknownError, "An unknown error occurred during the payment process." }
    };

        public static string GetMessage(string resultCode)
        {
            return Messages.TryGetValue(resultCode, out var message) ? message : "An unspecified error occurred.";
        }
    }
}
