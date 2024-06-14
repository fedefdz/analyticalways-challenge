namespace AcmeSchool.Domain.Exceptions
{
    public class StudentAgeInsuffiicientException : DomainException
    {
        public StudentAgeInsuffiicientException()
            : base((int)DomainErrorCodes.StudentAgeInsuffcient, "student does not meet the age requirement.")
        { }
    }
}
