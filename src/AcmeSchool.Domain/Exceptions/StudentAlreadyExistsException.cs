namespace AcmeSchool.Domain.Exceptions
{
    public class StudentAlreadyExistsException : DomainException
    {
        public StudentAlreadyExistsException() : 
            base((int) DomainErrorCodes.StudentAlreadyExists, "student already exists")
        { }
    }
}
