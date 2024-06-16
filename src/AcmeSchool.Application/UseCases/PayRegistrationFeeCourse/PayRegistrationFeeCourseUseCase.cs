using AcmeSchool.Application.Services.PaymentGateway;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using AcmeSchool.Domain.ValueObjects;

namespace AcmeSchool.Application.UseCases.PayRegistrationFeeCourse
{
    public interface IPayRegistrationFeeUseCase
    {
        Task<CourseRegistrationFeePayment> ExecuteAsync(PayRegistrationFeeCourseCommand command);
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

        public async Task<CourseRegistrationFeePayment> ExecuteAsync(PayRegistrationFeeCourseCommand command)
        {
            command.ValidateIfFailThrow();

            Course course = await _courseRepository.GetByIdOrDefaultAsync(command.CourseId)
                ?? throw new CourseNotFoundException();

            Student student = await _studentRepository.GetByIdOrDefaultAsync(command.StudentId)
                ?? throw new StudentNotFoundException();

            CourseRegistrationFeePayment registrationFeePayment = await _paymentRepository.GetCourseRegistrationFeePaymentByIdOrDefaultAsync(command.RegistrationFeePaymentRequest.PaymentId)
                ?? throw new CourseRegistrationFeePaymentNotFoundException();

            if (command.RegistrationFeePaymentRequest.Amount < course.RegistrationFee)
                throw new PaymentAmountInsufficientException();

            if (registrationFeePayment.Status != PaymentStatus.Pending) throw new OperationNotAllowedException("payment is not pending");
            if (course.Id != registrationFeePayment.CourseId) throw new OperationNotAllowedException("payment is not for this course");
            if (student.Id != registrationFeePayment.StudentId) throw new OperationNotAllowedException("payment is not for this student");


            PaymentResult paymentResult = await _paymentGateway.ProcessPaymentAsync(command.RegistrationFeePaymentRequest);
            
            ProcessPaymentResult(paymentResult, registrationFeePayment);
            
            if (registrationFeePayment.Status == PaymentStatus.Approved)
            {
                course.PayRegistrationFee(registrationFeePayment);
                await _courseRepository.UpdateAsync(course);
            }

            await _paymentRepository.UpdateCourseRegistrationFeePaymentAsync(registrationFeePayment);
            return registrationFeePayment;
        }

        private void ProcessPaymentResult(PaymentResult paymentResult, CourseRegistrationFeePayment registrationFeePayment)
        {
            if (paymentResult.Success)
            {
                registrationFeePayment.Approbe(paymentResult.ApprovationCode);
                return;
            }

            if (paymentResult.ResultCode == PaymentResultCodes.InsufficientFunds)
            {
                registrationFeePayment.Reject();
            }
            else
            {
                registrationFeePayment.Fail();
            }
        }
    }
}
