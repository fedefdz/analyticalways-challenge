using System.Runtime.Serialization;

namespace AcmeSchool.Domain.Exceptions
{
    public class StudentRegistrationFeeNotPaidException : DomainException
    {
        public StudentRegistrationFeeNotPaidException() : 
            base((int) DomainErrorCodes.StudentRegistrationFeeNotPaid, "student registration fee not paid")
        {
        }
    }
}