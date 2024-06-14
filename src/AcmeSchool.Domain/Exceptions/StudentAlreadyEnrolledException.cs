namespace AcmeSchool.Domain.Exceptions
{
    public class StudentAlreadyEnrolledException : DomainException
    {
        public StudentAlreadyEnrolledException(string courseName) 
            : base((int) DomainErrorCodes.StudentAlreadyEnrolled, $"student already enrolled in {courseName}")
        {
        }
    }
}