using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.ValueObjects;

namespace AcmeSchool.Domain.Entities
{
    public class CourseRegitrationFeePayment
    {
        public CourseRegitrationFeePayment(Guid courseId, Guid studentId, decimal amount, PaymentMethod paymentMethod)
        {
            PaymentId = Guid.NewGuid();
            CourseId = courseId;
            StudentId = studentId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Status = PaymentStatus.Pending;
        }

        public Guid PaymentId { get; private set; }
        public Guid CourseId { get; private set; }
        public Guid StudentId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime? PaymentDate { get; private set; }
        public string? ApprobationCode { get; private set; }

        public void Approbe(string approbationCode)
        {            
            ChangeStatus(PaymentStatus.Approved);
            ApprobationCode = approbationCode;
            PaymentDate = DateTime.Now;
        }

        public void Reject()
        {
            ChangeStatus(PaymentStatus.Rejected);
        }

        public void Fail()
        {
            ChangeStatus(PaymentStatus.Failed);
        }

        private void ChangeStatus(PaymentStatus status)
        {
            if (Status != PaymentStatus.Pending) throw new OperationNotAllowedException("payment is not pending");
            Status = status;
        }
    }
}