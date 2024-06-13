namespace AcmeSchool.Domain.Exceptions
{
    public class StudentInvalidDataException : DomainException
    {
        public StudentInvalidDataException() : 
            base((int) DomainErrorCodes.StudentInvalidData, "student data is invalid")
        { }

        public StudentInvalidDataException(string fieldName) :
            base((int)DomainErrorCodes.StudentInvalidData, $"student '{fieldName}' is invalid")
        { }
    }
}
