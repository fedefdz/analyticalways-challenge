namespace AcmeSchool.Domain.Exceptions
{
    public class CourseRegistrationFeePaymentNotFoundException : DomainException
    {
        public CourseRegistrationFeePaymentNotFoundException() : 
            base((int) DomainErrorCodes.CourseResgistrationFeePaymentNotFound, "course payment not found")
        { }
    }
}
