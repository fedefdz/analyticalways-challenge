namespace AcmeSchool.Domain.ValueObjects
{
    public class ResgitrationFeePayment
    {
        public Guid PaymentId { get; private set; }
        public Guid StudentId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }

        public ResgitrationFeePayment(Guid paymentId, Guid studentId, decimal amount, DateTime paymentDate, PaymentMethod paymentMethod)
        {
            PaymentId = paymentId;
            StudentId = studentId;
            Amount = amount;
            PaymentDate = paymentDate;
            PaymentMethod = paymentMethod;
        }
    }
}