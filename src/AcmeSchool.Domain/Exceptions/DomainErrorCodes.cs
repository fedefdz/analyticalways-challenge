namespace AcmeSchool.Domain.Exceptions
{
    public enum DomainErrorCodes
    {
        StudentNotFound = 1001,
        StudentAlreadyExists = 1002,
        StudentInvalidData = 1003,
        StudentAgeInsuffcient = 1004,

        CourseNotFound = 2001,
        CourseAlreadyExists = 2002,
        CourseInvalidData = 2003,

        PaymentInvalidData = 3003,
        PaymentAmountInsufficient = 3004
    }
}
