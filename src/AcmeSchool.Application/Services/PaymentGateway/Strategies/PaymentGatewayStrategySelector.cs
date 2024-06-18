using AcmeSchool.Application.Services.PaymentGateway.Strategies.BankTransfer;
using AcmeSchool.Application.Services.PaymentGateway.Strategies.CreditCard;
using AcmeSchool.Application.Services.PaymentGateway.Strategies.DebitCard;

namespace AcmeSchool.Application.Services.PaymentGateway.Strategies
{
    public class PaymentGatewayStrategySelector
    {
        private readonly IPaymentGatewayDebitCardStrategy _debitCardStrategy;
        private readonly IPaymentGatewayCreditCardStrategy _creditCardStrategy;
        private readonly IPaymentGatewayBankTransferSatrategy _bankTransferStrategy;

        public PaymentGatewayStrategySelector(
            IPaymentGatewayDebitCardStrategy debitCardStrategy,
            IPaymentGatewayCreditCardStrategy creditCardStrategy,
            IPaymentGatewayBankTransferSatrategy bankTransferStrategy)
        {
            _debitCardStrategy = debitCardStrategy;
            _creditCardStrategy = creditCardStrategy;
            _bankTransferStrategy = bankTransferStrategy;
        }

        public IPaymentGatewayStrategy<TPaymentRequest> SelectStrategy<TPaymentRequest>(TPaymentRequest paymentRequest) where TPaymentRequest : PaymentRequest
        {
            return paymentRequest switch
            {
                DebitCardPaymentRequest _ => (IPaymentGatewayStrategy<TPaymentRequest>)_debitCardStrategy,
                CreditCardPaymentRequest _ => (IPaymentGatewayStrategy<TPaymentRequest>)_creditCardStrategy,
                BankTransferPaymentRequest _ => (IPaymentGatewayStrategy<TPaymentRequest>)_bankTransferStrategy,
                _ => throw new ArgumentException("unsupported payment request type", paymentRequest.GetType().Name),
            };
        }
    }
}
