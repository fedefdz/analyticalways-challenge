using AcmeSchool.Application.UseCases.Common;
using AcmeSchool.Domain.Exceptions;

namespace AcmeSchool.Application.UseCases.EnrollStudentInCourse
{
    public record class EnrollStudentInCourseCommand(Guid CourseId, Guid StudentId) : UseCaseCommand
    {
        public override void ValidateIfFailThrow()
        {
            if (CourseId == Guid.Empty) throw new CourseInvalidDataException(nameof(CourseId), "could not be empty.");
            if (StudentId == Guid.Empty) throw new StudentInvalidDataException(nameof(StudentId), "could not be empty.");
        }
    }
}
