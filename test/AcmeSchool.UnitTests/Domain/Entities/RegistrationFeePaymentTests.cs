using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.ValueObjects;
using FluentAssertions;

namespace AcmeSchool.UnitTests.Domain.Entities
{
    public class RegistrationFeePaymentTests
    {
    }

    public class RegistrationFeePaymentConstructorTests
    {
        [Fact]
        public void Constructor_ShouldSetPaymentStatusToPending()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var amount = 100m;
            var paymentMethod = PaymentMethod.CreditCard;

            // Act
            var payment = new RegistrationFeePayment(paymentId, studentId, amount, paymentMethod);

            // Assert            
            payment.Status.Should().Be(PaymentStatus.Pending);
            payment.PaymentDate.Should().BeNull();
            payment.ApprobationCode.Should().BeNull();
        }
    }
}
