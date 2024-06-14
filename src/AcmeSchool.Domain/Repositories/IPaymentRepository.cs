using AcmeSchool.Domain.Entities;

namespace AcmeSchool.Domain.Repositories
{
    public interface IPaymentRepository
    {
        Task AddCourseRegistrationFeePayementAsync(CourseRegitrationFeePayment courseRegitrationFeePayment);
        Task<CourseRegitrationFeePayment> GetCourseRegistrationFeePaymentByIdOrDefaultAsync(Guid id);
        Task UpdateCourseRegistrationFeePaymentAsync(CourseRegitrationFeePayment courseRegitrationFeePayment);
    }
}
