using AcmeSchool.Application.Services.PaymentGateway;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using AcmeSchool.Domain.ValueObjects;

namespace AcmeSchool.Application.UseCases.PayRegistrationFeeCourse
{
    public interface IPayRegistrationFeeUseCase
    {
        Task<RegistrationFeePayment> ExecuteAsync(PayRegistrationFeeCourseCommand command);
    }

    public class PayRegistrationFeeCourseUseCase : IPayRegistrationFeeUseCase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentGatewayService _paymentGateway;

        public PayRegistrationFeeCourseUseCase(ICourseRepository courseRepository, IStudentRepository studentRepository, IPaymentRepository paymentRepository, IPaymentGatewayService paymentGateway)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _paymentGateway = paymentGateway ?? throw new ArgumentNullException(nameof(paymentGateway));
        }

        public async Task<RegistrationFeePayment> ExecuteAsync(PayRegistrationFeeCourseCommand command)
        {
            command.ValidateIfFailThrow();

            Course course = await GetCourseOrThrow(command.CourseId);
            Student student = await GetStudentOrThrow(command.StudentId);
            RegistrationFeePayment registrationFeePayment = await GetRegistrationFeePaymentOrThrow(command.RegistrationFeePaymentRequest.PaymentId);

            ValidatePaymentConditions(command.RegistrationFeePaymentRequest, course, student, registrationFeePayment);
            
            PaymentResult paymentResult = await _paymentGateway.ProcessPaymentAsync(command.RegistrationFeePaymentRequest);
            
            await ProcessPaymentResult(paymentResult, registrationFeePayment);
            
            if (registrationFeePayment.Status == PaymentStatus.Approved)
            {
                await ConfirmRegistrationFeePayment(course, registrationFeePayment);                
            }

            
            return registrationFeePayment;
        }

        private async Task ProcessPaymentResult(PaymentResult paymentResult, RegistrationFeePayment registrationFeePayment)
        {
            switch (paymentResult.ResultCode)
            {
                case PaymentResultCodes.Success:
                    registrationFeePayment.Approve(paymentResult.ApprovationCode!);
                    break;
                case PaymentResultCodes.InsufficientFunds:
                    registrationFeePayment.Reject();
                    break;
                default:
                    registrationFeePayment.Fail();
                    break;
            }

            await _paymentRepository.UpdateCourseRegistrationFeePaymentAsync(registrationFeePayment);
        }

        private async Task ConfirmRegistrationFeePayment(Course course, RegistrationFeePayment registrationFeePayment)
        {
            course.PayRegistrationFee(registrationFeePayment);
            await _courseRepository.UpdateAsync(course);
        }

        private void ValidatePaymentConditions(PaymentRequest paymentRequest, Course course, Student student, RegistrationFeePayment registrationFeePayment)
        {
            if (paymentRequest.Amount < course.RegistrationFee)
                throw new PaymentAmountInsufficientException();

            if (registrationFeePayment.Status != PaymentStatus.Pending) throw new OperationNotAllowedException("payment is not pending");
            if (course.Id != registrationFeePayment.CourseId) throw new OperationNotAllowedException("payment is not for this course");
            if (student.Id != registrationFeePayment.StudentId) throw new OperationNotAllowedException("payment is not for this student");
        }

        private async Task<Course> GetCourseOrThrow(Guid courseId) 
            => await _courseRepository.GetByIdOrDefaultAsync(courseId) ?? throw new CourseNotFoundException();

        private async Task<Student> GetStudentOrThrow(Guid studentId) 
            => await _studentRepository.GetByIdOrDefaultAsync(studentId) ?? throw new StudentNotFoundException();

        private async Task<RegistrationFeePayment> GetRegistrationFeePaymentOrThrow(Guid paymentId) 
            => await _paymentRepository.GetCourseRegistrationFeePaymentByIdOrDefaultAsync(paymentId) ?? throw new CourseRegistrationFeePaymentNotFoundException();
    }
}
