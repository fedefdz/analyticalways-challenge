namespace AcmeSchool.Domain.Exceptions
{
    public class CourseAlreadyExistsException : DomainException
    {
        public CourseAlreadyExistsException() : 
            base((int) DomainErrorCodes.CourseAlreadyExists, "course already exists")
        { }
    }
}
