using AcmeSchool.Application.Services.PaymentGateway.DTO;

namespace AcmeSchool.Application.Services.PaymentGateway
{
    public interface IPaymentGatewayStrategy
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest);
    }
}
