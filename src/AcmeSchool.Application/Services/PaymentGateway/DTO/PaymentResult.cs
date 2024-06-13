namespace AcmeSchool.Application.Services.PaymentGateway.DTO
{
    public record PaymentResult(bool Success, string Message, string ResultCode, string ApprovationCode, string OperationNumber)
    {
        public PaymentResult(string resultCode, string approvationCode, string operationNumber)
            : this(resultCode == PaymentResultCodes.Success, PaymentResultMessages.GetMessage(resultCode), resultCode, approvationCode, operationNumber)
        { }
    }
}
