using AcmeSchool.Application.Services.PaymentGateway.DTO;

namespace AcmeSchool.Application.Services.PaymentGateway
{
    public interface IPaymentGatewayService<TPaymentRequest> where TPaymentRequest : PaymentRequest
    {
        Task<PaymentResult> ProcessPaymentAsync(TPaymentRequest paymentRequest);
    }
}
