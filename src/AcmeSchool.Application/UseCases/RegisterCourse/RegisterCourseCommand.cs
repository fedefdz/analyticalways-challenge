using AcmeSchool.Application.UseCases.Common;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;

namespace AcmeSchool.Application.UseCases.RegisterCourse
{
    public record RegisterCourseCommand(string Name, decimal RegistrationFee, DateTime StartDate, DateTime EndDate) : UseCaseCommand
    {
        public override void ValidateIfFailThrow()
        {
            _ = new Course(Name, RegistrationFee, StartDate, EndDate);

            if (StartDate.Date < DateTime.Now.Date) throw new CourseInvalidDataException(nameof(StartDate), "could not be in the past");
        }
    }
}
