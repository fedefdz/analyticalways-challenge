using AcmeSchool.Application.UseCases.Common;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.ValueObjects;

namespace AcmeSchool.Application.UseCases.ContractCourse
{
    public record ContractCourseCommand(Guid CourseId, Guid StudentId, PaymentMethod PaymentMethod) : UseCaseCommand 
    {
        public override void ValidateIfFailThrow()
        {
            if (CourseId == Guid.Empty) throw new CourseInvalidDataException(nameof(CourseId), "could not be empty.");
            if (StudentId == Guid.Empty) throw new StudentInvalidDataException(nameof(StudentId), "could not be empty.");
        }
    }
}