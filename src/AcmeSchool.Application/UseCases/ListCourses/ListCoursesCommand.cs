using AcmeSchool.Application.UseCases.Common;
using AcmeSchool.Domain.Exceptions;

namespace AcmeSchool.Application.UseCases.ListCourses
{
    public record ListCoursesCommand(DateTime FromDate, DateTime EndDate) : UseCaseCommand
    {
        public override void ValidateIfFailThrow()
        {
            if(FromDate.Date > EndDate.Date) throw new OperationNotAllowedException($"{nameof(FromDate)} could not be greater than {nameof(EndDate)}.");
        }
    }
}