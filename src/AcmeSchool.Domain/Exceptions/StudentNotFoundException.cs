namespace AcmeSchool.Domain.Exceptions
{
    public class StudentNotFoundException : DomainException
    {
        public StudentNotFoundException() : 
            base((int) DomainErrorCodes.StudentNotFound, "student not found")
        { }
    }
}
