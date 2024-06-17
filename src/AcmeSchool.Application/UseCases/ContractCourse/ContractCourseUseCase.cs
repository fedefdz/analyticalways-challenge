using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;

namespace AcmeSchool.Application.UseCases.ContractCourse
{
    public interface IContractCourseUseCase
    {
        public Task<RegistrationFeePayment> ExecuteAsync(ContractCourseCommand command);
    }

    public class ContractCourseUseCase : IContractCourseUseCase
    { 
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IPaymentRepository _paymentRepository;

        public ContractCourseUseCase(IStudentRepository studentRepository, ICourseRepository courseRepository, IPaymentRepository paymentRepository)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async Task<RegistrationFeePayment> ExecuteAsync(ContractCourseCommand command)
        {
            command.ValidateIfFailThrow();

            Course course = await GetCourseOrThrow(command.CourseId);
            Student student = await GetStudentOrThrow(command.StudentId);

            if (course.IsStudentEnrolled(student)) throw new OperationNotAllowedException("student already enrolled in course.");
            if (course.HasStudentRegitrationFeePaid(student)) throw new OperationNotAllowedException("student already paid registration fee.");

            var payment = RegistrationFeePayment.CreatePendingPayment(course.Id, student.Id, course.RegistrationFee, command.PaymentMethod);
            await _paymentRepository.AddCourseRegistrationFeePaymentAsync(payment);

            return payment;
        }

        private async Task<Course> GetCourseOrThrow(Guid courseId) => await _courseRepository.GetByIdOrDefaultAsync(courseId) ?? throw new CourseNotFoundException();

        private async Task<Student> GetStudentOrThrow(Guid studentId) => await _studentRepository.GetByIdOrDefaultAsync(studentId) ?? throw new StudentNotFoundException();
    }
}
