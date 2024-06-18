using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.ValueObjects;
using AcmeSchool.UnitTests.Common;
using AutoFixture;
using FluentAssertions;

namespace AcmeSchool.UnitTests.Domain.Entities
{
    public class RegistrationFeePaymentTests
    {
        private readonly IFixture _fixture = new Fixture();

        [Theory, AutoMoqData]
        public void Approve_ShouldSetPaymentStatusToApproved(string approvationCode)
        {
            // Arrange
            RegistrationFeePayment payment = CreatePendingPayment();
            var timesatmpBeforeApprove = DateTime.UtcNow;

            // Act
            payment.Approve(approvationCode);
            var timesatmpAfterApprove = DateTime.UtcNow;

            // Assert
            payment.Status.Should().Be(PaymentStatus.Approved);
            payment.ApprovationCode.Should().Be(approvationCode);
            payment.PaymentDate.Should().NotBeNull();
            payment.PaymentDate.Should().BeOnOrAfter(timesatmpBeforeApprove).And.BeOnOrBefore(timesatmpAfterApprove);
            payment.PaymentDate!.Value.Kind.Should().Be(DateTimeKind.Utc);            
        }

        [Theory, AutoMoqData]
        public void Approve_WhenStatusIsNotPendind_ThrowOperationNotAllowedException(string approvationCode)
        {
            // Arrange
            RegistrationFeePayment payment = CreateNotPendingPayment();
            
            // Act
            Action act = () => payment.Approve(approvationCode);
            
            // Assert
            act.Should().Throw<OperationNotAllowedException>();
        }

        [Fact]
        public void Reject_ShouldSetPaymentStatusToRejected()
        {
            // Arrange
            RegistrationFeePayment payment = CreatePendingPayment();

            // Act
            payment.Reject();

            // Assert
            payment.Status.Should().Be(PaymentStatus.Rejected);
            payment.ApprovationCode.Should().BeNull();
            payment.PaymentDate.Should().BeNull();
        }

        [Fact]
        public void Reject_WhenStatusIsNotPendind_ThrowOperationNotAllowedException()
        {
            // Arrange
            RegistrationFeePayment payment = CreateNotPendingPayment();

            // Act
            Action act = () => payment.Reject();

            // Assert
            act.Should().Throw<OperationNotAllowedException>();
        }

        [Fact]
        public void Fail_ShouldSetPaymentStatusToFailed()
        {
            // Arrange
            RegistrationFeePayment payment = CreatePendingPayment();

            // Act
            payment.Fail();

            // Assert
            payment.Status.Should().Be(PaymentStatus.Failed);
            payment.ApprovationCode.Should().BeNull();
            payment.PaymentDate.Should().BeNull();
        }

        [Fact]
        public void Fail_WhenStatusIsNotPendind_ThrowOperationNotAllowedException()
        {
            // Arrange
            RegistrationFeePayment payment = CreateNotPendingPayment();

            // Act
            Action act = () => payment.Fail();

            // Assert
            act.Should().Throw<OperationNotAllowedException>();
        }

        private RegistrationFeePayment CreatePendingPayment()
        {
            return RegistrationFeePayment.CreatePendingPayment(
                courseId: _fixture.Create<Guid>(),
                studentId: _fixture.Create<Guid>(),
                registrationFee: _fixture.Create<decimal>(),
                paymentMethod: _fixture.Create<PaymentMethod>());
        }

        private RegistrationFeePayment CreateNotPendingPayment()
        {
            var payment = RegistrationFeePayment.CreatePendingPayment(
                courseId: _fixture.Create<Guid>(),
                studentId: _fixture.Create<Guid>(),
                registrationFee: _fixture.Create<decimal>(),
                paymentMethod: _fixture.Create<PaymentMethod>());

            var status = _fixture.Create<Generator<PaymentStatus>>()
                .Where(status => status != PaymentStatus.Pending)
                .First();

            Setter.SetProperty(payment, nameof(RegistrationFeePayment.Status), status);
            return payment;

        }
    }

    public class RegistrationFeePaymentConstructorTests
    {
        [Theory, AutoMoqData]
        public void Constructor_ShouldSetPaymentStatusToPending(Guid paymentId, Guid studentId, decimal amount, PaymentMethod paymentMethod)
        {
            // Act
            var payment = new RegistrationFeePayment(paymentId, studentId, amount, paymentMethod);

            // Assert            
            payment.Status.Should().Be(PaymentStatus.Pending);
            payment.PaymentDate.Should().BeNull();
            payment.ApprovationCode.Should().BeNull();
        }
    }
}
