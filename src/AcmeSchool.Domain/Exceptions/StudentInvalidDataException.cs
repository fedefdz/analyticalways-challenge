namespace AcmeSchool.Domain.Exceptions
{
    public class StudentInvalidDataException : DomainException
    {
        public StudentInvalidDataException(string fieldName) :
            base((int)DomainErrorCodes.StudentInvalidData, $"student '{fieldName}' is invalid")
        { }

        public StudentInvalidDataException(string fieldName, string message) :
            base((int)DomainErrorCodes.StudentInvalidData, $"student '{fieldName}' {message}")            
        { }
    }
}
