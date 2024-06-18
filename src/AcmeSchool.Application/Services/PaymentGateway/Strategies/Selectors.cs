using AcmeSchool.Application.Services.PaymentGateway.Strategies.BankTransfer;
using AcmeSchool.Application.Services.PaymentGateway.Strategies.CreditCard;
using AcmeSchool.Application.Services.PaymentGateway.Strategies.DebitCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public IPaymentGatewayStrategy<PaymentRequest> SelectStrategy(PaymentRequest paymentRequest)
        {
            switch (paymentRequest)
            {
                case DebitCardPaymentRequest _:
                    return _debitCardStrategy;
                case CreditCardPaymentRequest _:
                    return _creditCardStrategy;
                case BankTransferPaymentRequest _:
                    return _bankTransferStrategy;
                default:
                    throw new ArgumentException("Unsupported payment request type", nameof(paymentRequest));
            }
        }
    }

    public class PaymentGatewayStrategySelector
    {
        private readonly Dictionary<Type, IPaymentGatewayStrategy> _strategies = new();

        public void RegisterStrategy<TPaymentRequest>(IPaymentGatewayStrategy<TPaymentRequest> strategy)
            where TPaymentRequest : PaymentRequest
        {
            _strategies[typeof(TPaymentRequest)] = strategy;
        }

        public IPaymentGatewayStrategy<TPaymentRequest> GetStrategy<TPaymentRequest>()
            where TPaymentRequest : PaymentRequest
        {
            if (_strategies.TryGetValue(typeof(TPaymentRequest), out var strategy))
            {
                return (IPaymentGatewayStrategy<TPaymentRequest>)strategy;
            }

            throw new ArgumentException($"No strategy registered for type {typeof(TPaymentRequest).Name}");
        }
    }
}
