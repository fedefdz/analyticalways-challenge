using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;

namespace AcmeSchool.Application.UseCases.ContractCourse
{
    public interface IContractCourseUseCase
    {
        public Task<CourseRegitrationFeePayment> ExcecuteAsync(ContractCourseCommand command);
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

        public async Task<CourseRegitrationFeePayment> ExcecuteAsync(ContractCourseCommand command)
        {
            command.ValidateIfFailThrow();

            Course course = await _courseRepository.GetByIdOrDefaultAsync(command.CourseId)
                ?? throw new CourseNotFoundException();

            Student student = await _studentRepository.GetByIdOrDefaultAsync(command.StudentId)
                ?? throw new StudentNotFoundException();

            if (course.IsStudentEnrolled(student)) throw new OperationNotAllowedException("student already enrolled in course.");
            if (course.HasStudentRegitrationFeePaid(student)) throw new OperationNotAllowedException("student already paid registration fee.");

            var payment = new CourseRegitrationFeePayment(course.Id, student.Id, course.RegistrationFee, command.paymentMethod);
            await _paymentRepository.AddCourseRegistrationFeePayementAsync(payment);

            return payment;
        }        
    }


}
