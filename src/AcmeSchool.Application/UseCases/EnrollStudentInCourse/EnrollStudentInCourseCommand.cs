using AcmeSchool.Application.UseCases.Common;
using AcmeSchool.Domain.Exceptions;

namespace AcmeSchool.Application.UseCases.EnrollStudentInCourse
{
    public record class EnrollStudentInCourseCommand(string CourseName, string StudentName) : UseCaseCommand
    {
        public override void ValidateIfFailThrow()
        {
            if (string.IsNullOrWhiteSpace(CourseName)) throw new CourseInvalidDataException(nameof(CourseName), "could not be empty.");
            if (string.IsNullOrWhiteSpace(StudentName)) throw new StudentInvalidDataException(nameof(StudentName), "could not be empty.");
        }
    }
}
