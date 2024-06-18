namespace AcmeSchool.Application.Services.PaymentGateway.Strategies
{
    public class PaymentGatewayStrategyDynamicSelector
    {
        private readonly Dictionary<Type, object> _strategies = [];

        public void RegisterStrategy<TPaymentRequest>(IPaymentGatewayStrategy<TPaymentRequest> strategy)
            where TPaymentRequest : PaymentRequest
        {
            // always override the strategy
            _strategies[typeof(TPaymentRequest)] = strategy;
        }

        public IPaymentGatewayStrategy<TPaymentRequest> SelectStrategy<TPaymentRequest>(TPaymentRequest paymentRequest)
            where TPaymentRequest : PaymentRequest
        {
            if (_strategies.TryGetValue(paymentRequest.GetType(), out var strategy))
            {
                return (IPaymentGatewayStrategy<TPaymentRequest>)strategy;
            }

            throw new ArgumentException("no strategy registered for type", paymentRequest.GetType().Name);
        }
    }
}
