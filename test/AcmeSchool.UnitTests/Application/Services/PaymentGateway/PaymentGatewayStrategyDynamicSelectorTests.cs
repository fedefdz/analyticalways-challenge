using AcmeSchool.Application.Services.PaymentGateway.Strategies.CreditCard;
using AcmeSchool.Application.Services.PaymentGateway.Strategies.DebitCard;
using AcmeSchool.Application.Services.PaymentGateway.Strategies;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace AcmeSchool.UnitTests.Application.Services.PaymentGateway
{
    public  class PaymentGatewayStrategyDynamicSelectorTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void RegisterStrategy_ThenSelectStrategy_ReturnsRegisteredStrategy()
        {
            // Arrange
            PaymentGatewayStrategyDynamicSelector _selector = new PaymentGatewayStrategyDynamicSelector();
            var strategyMock = new Mock<IPaymentGatewayStrategy<DebitCardPaymentRequest>>();            
            _selector.RegisterStrategy(strategyMock.Object);
            DebitCardPaymentRequest paymentRequest = _fixture.Create<DebitCardPaymentRequest>();

            // Act
            var result = _selector.SelectStrategy(paymentRequest);

            // Assert
            result.Should().BeSameAs(strategyMock.Object);
            result.Should().BeAssignableTo(typeof(IPaymentGatewayStrategy<DebitCardPaymentRequest>));
        }

        [Fact]
        public void SelectStrategy_WithUnregisteredPaymentRequest_ThrowsArgumentException()
        {
            // Arrange
            PaymentGatewayStrategyDynamicSelector _selector = new PaymentGatewayStrategyDynamicSelector();
            var paymentRequest = _fixture.Create<CreditCardPaymentRequest>();

            // Act
            Action act = () => _selector.SelectStrategy(paymentRequest);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("no strategy registered for type*");
        }

        [Fact]
        public void RegisterStrategy_OverridesPreviousStrategy_WhenSameTypeRegisteredAgain()
        {
            // Arrange
            PaymentGatewayStrategyDynamicSelector _selector = new PaymentGatewayStrategyDynamicSelector();
            var firstStrategyMock = new Mock<IPaymentGatewayStrategy<DebitCardPaymentRequest>>();
            var secondStrategyMock = new Mock<IPaymentGatewayStrategy<DebitCardPaymentRequest>>();
            _selector.RegisterStrategy(firstStrategyMock.Object);
            _selector.RegisterStrategy(secondStrategyMock.Object);

            DebitCardPaymentRequest paymentRequest = _fixture.Create<DebitCardPaymentRequest>();

            // Act
            var result = _selector.SelectStrategy(paymentRequest);

            // Assert
            result.Should().BeSameAs(secondStrategyMock.Object);
            result.Should().BeAssignableTo(typeof(IPaymentGatewayStrategy<DebitCardPaymentRequest>));
        }
    }
}
