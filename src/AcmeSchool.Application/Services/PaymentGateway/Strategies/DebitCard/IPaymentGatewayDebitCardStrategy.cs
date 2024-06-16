using AcmeSchool.Application.Services.PaymentGateway.Strategies;

namespace AcmeSchool.Application.Services.PaymentGateway.Strategies.DebitCard
{
    public interface IPaymentGatewayDebitCardStrategy : IPaymentGatewayStrategy<DebitCardPaymentRequest>
    {
    }
}
