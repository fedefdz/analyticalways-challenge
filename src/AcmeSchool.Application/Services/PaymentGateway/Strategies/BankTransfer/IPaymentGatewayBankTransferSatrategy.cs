using AcmeSchool.Application.Services.PaymentGateway.Strategies;

namespace AcmeSchool.Application.Services.PaymentGateway.Strategies.BankTransfer
{
    public interface IPaymentGatewayBankTransferSatrategy : IPaymentGatewayStrategy<BankTransferPaymentRequest>
    {
    }
}
