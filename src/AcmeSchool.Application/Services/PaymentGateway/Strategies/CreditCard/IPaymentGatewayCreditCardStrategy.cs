using AcmeSchool.Application.Services.PaymentGateway.Strategies;

namespace AcmeSchool.Application.Services.PaymentGateway.Strategies.CreditCard
{
    public interface IPaymentGatewayCreditCardStrategy : IPaymentGatewayStrategy<CreditCardPaymentRequest>
    {
    }
}
