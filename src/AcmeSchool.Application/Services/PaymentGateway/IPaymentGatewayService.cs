namespace AcmeSchool.Application.Services.PaymentGateway
{
    public interface IPaymentGatewayService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest);
    }
}
