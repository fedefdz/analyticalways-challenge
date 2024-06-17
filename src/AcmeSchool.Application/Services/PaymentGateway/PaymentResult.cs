namespace AcmeSchool.Application.Services.PaymentGateway
{
    public record PaymentResult(Guid PaymentId, bool Success, string Message, string ResultCode, string OperationNumber, string? ApprovationCode)
    {
        public PaymentResult(Guid paymentId, string resultCode, string operationNumber, string? approvationCode)
            : this(paymentId, resultCode == PaymentResultCodes.Success, PaymentResultMessages.GetMessage(resultCode), resultCode, operationNumber, approvationCode)
        { }
    }
}
