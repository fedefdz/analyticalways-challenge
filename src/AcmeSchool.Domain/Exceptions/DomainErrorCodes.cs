namespace AcmeSchool.Domain.Exceptions
{
    public enum DomainErrorCodes
    {
        StudentAlreadyExists = 1001,
        StudentInvalidData = 1002,
        StudentAgeInsuffcient = 1003,

        CourseAlreadyExists = 2001,
        CourseInvalidData = 2002
    }
}
