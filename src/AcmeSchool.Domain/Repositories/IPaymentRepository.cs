using AcmeSchool.Domain.Entities;

namespace AcmeSchool.Domain.Repositories
{
    public interface IPaymentRepository
    {
        Task AddCourseRegistrationFeePaymentAsync(CourseRegistrationFeePayment courseRegitrationFeePayment);
        Task<CourseRegistrationFeePayment> GetCourseRegistrationFeePaymentByIdOrDefaultAsync(Guid id);
        Task UpdateCourseRegistrationFeePaymentAsync(CourseRegistrationFeePayment courseRegitrationFeePayment);
    }
}
