namespace AcmeSchool.Domain.Exceptions
{
    public class StudentAgeInsuffcientException : DomainException
    {
        public StudentAgeInsuffcientException()
            : base((int)DomainErrorCodes.StudentAgeInsuffcient, "student does not meet the age requirement.")
        { }
    }
}
