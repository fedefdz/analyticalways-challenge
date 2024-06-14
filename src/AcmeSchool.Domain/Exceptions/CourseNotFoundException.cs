namespace AcmeSchool.Domain.Exceptions
{
    public class CourseNotFoundException : DomainException
    {
        public CourseNotFoundException() : 
            base((int) DomainErrorCodes.CourseNotFound, "course not found")
        { }
    }
}
