namespace AcmeSchool.Domain.Exceptions
{
    public class CourseInvalidDataException : DomainException
    {
        public CourseInvalidDataException(string fieldName) :
            base((int)DomainErrorCodes.CourseInvalidData, $"course '{fieldName}' is invalid")
        { }

        public CourseInvalidDataException(string fieldName, string message) :
            base((int)DomainErrorCodes.CourseInvalidData, $"course '{fieldName}' {message}")
        { }
    }
}
