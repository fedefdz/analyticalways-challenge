namespace AcmeSchool.Application.Services.PaymentGateway.Strategies
{
    public interface IPaymentGatewayStrategy<TPaymentRequest> where TPaymentRequest : PaymentRequest
    {
        Task<PaymentResult> ProcessPaymentAsync(TPaymentRequest paymentRequest);
    }
}
