using AcmeSchool.Application.Services.PaymentGateway.DTO;

namespace AcmeSchool.Application.Services.PaymentGateway
{
    public interface IPaymentGatewayCreditCardStrategy : IPaymentGatewayStrategy<CreditCardPaymentRequest>
    {
    }
}
