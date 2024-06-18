using AcmeSchool.Application.Services.PaymentGateway.Strategies.BankTransfer;
using AcmeSchool.Application.Services.PaymentGateway.Strategies.CreditCard;
using AcmeSchool.Application.Services.PaymentGateway.Strategies.DebitCard;
using AcmeSchool.Application.Services.PaymentGateway.Strategies;
using AcmeSchool.Application.Services.PaymentGateway;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace AcmeSchool.UnitTests.Application.Services.PaymentGateway
{
    public class PaymentGatewayStrategySelectorTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IPaymentGatewayDebitCardStrategy> _debitCardStrategyMock = new Mock<IPaymentGatewayDebitCardStrategy>();
        private readonly Mock<IPaymentGatewayCreditCardStrategy> _creditCardStrategyMock = new Mock<IPaymentGatewayCreditCardStrategy>();
        private readonly Mock<IPaymentGatewayBankTransferSatrategy> _bankTransferStrategyMock = new Mock<IPaymentGatewayBankTransferSatrategy>();
        private readonly PaymentGatewayStrategySelector _selector;

        public PaymentGatewayStrategySelectorTests()
        {
            _selector = new PaymentGatewayStrategySelector(
                _debitCardStrategyMock.Object,
                _creditCardStrategyMock.Object,
                _bankTransferStrategyMock.Object);
        }

        [Fact]
        public void SelectStrategy_WithDebitCardPaymentRequest_ReturnsDebitCardStrategy()
        {
            // Arrange
            var request = _fixture.Create<DebitCardPaymentRequest>();

            // Act
            var result = _selector.SelectStrategy(request);

            // Assert
            result.Should().BeAssignableTo(typeof(IPaymentGatewayDebitCardStrategy));
        }

        [Fact]
        public void SelectStrategy_WithCreditCardPaymentRequest_ReturnsCreditCardStrategy()
        {
            // Arrange
            var request = _fixture.Create<CreditCardPaymentRequest>();

            // Act
            var result = _selector.SelectStrategy(request);

            // Assert
            result.Should().BeAssignableTo(typeof(IPaymentGatewayCreditCardStrategy));
        }

        [Fact]
        public void SelectStrategy_WithBankTransferPaymentRequest_ReturnsBankTransferStrategy()
        {
            // Arrange
            var request = _fixture.Create<BankTransferPaymentRequest>();

            // Act
            var result = _selector.SelectStrategy(request);

            // Assert
            result.Should().BeAssignableTo(typeof(IPaymentGatewayBankTransferSatrategy));
        }

        [Fact]
        public void SelectStrategy_WithUnsupportedPaymentRequest_ThrowsArgumentException()
        {
            // Arrange
            var request = _fixture.Create<PaymentRequestUnsopported>();

            // Act
            Action act = () => _selector.SelectStrategy(request);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("unsupported payment request type*");
        }

        // this class is used to test the case when the payment request is not supported by any strategy
        internal record PaymentRequestUnsopported : PaymentRequest
        {}
    }
}
