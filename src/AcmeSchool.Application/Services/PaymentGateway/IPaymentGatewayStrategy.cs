using AcmeSchool.Application.Services.PaymentGateway.DTO;

namespace AcmeSchool.Application.Services.PaymentGateway
{
    public interface IPaymentGatewayStrategy<TPaymentRequest> where TPaymentRequest : PaymentRequest
    {
        Task<PaymentResult> ProcessPaymentAsync(TPaymentRequest paymentRequest);
    }
}
