using AcmeSchool.Domain.Entities;

namespace AcmeSchool.Domain.Repositories
{
    public interface IPaymentRepository
    {
        Task AddCourseRegistrationFeePaymentAsync(RegistrationFeePayment courseRegitrationFeePayment);
        Task<RegistrationFeePayment> GetCourseRegistrationFeePaymentByIdOrDefaultAsync(Guid id);
        Task UpdateCourseRegistrationFeePaymentAsync(RegistrationFeePayment courseRegitrationFeePayment);
    }
}
