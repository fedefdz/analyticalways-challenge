using AcmeSchool.Application.Services.PaymentGateway;
using AcmeSchool.Application.UseCases.Common;
using AcmeSchool.Domain.Exceptions;

namespace AcmeSchool.Application.UseCases.PayRegistrationFeeCourse
{
    public record PayRegistrationFeeCourseCommand(Guid CourseId, Guid StudentId, PaymentRequest RegistrationFeePaymentRequest) : UseCaseCommand
    {
        public override void ValidateIfFailThrow()
        {
            if (CourseId == Guid.Empty) throw new CourseInvalidDataException(nameof(CourseId), "could not be empty.");
            if (StudentId == Guid.Empty) throw new StudentInvalidDataException(nameof(StudentId), "could not be empty.");
            if (RegistrationFeePaymentRequest == null) throw new PaymentInvalidDataException(nameof(RegistrationFeePaymentRequest), "could not be null.");
        }
    }
}