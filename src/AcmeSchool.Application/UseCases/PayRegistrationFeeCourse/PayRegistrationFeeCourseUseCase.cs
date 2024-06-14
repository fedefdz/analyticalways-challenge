using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;

namespace AcmeSchool.Application.UseCases.PayRegistrationFeeCourse
{
    public interface IPayRegistrationFeeUseCase
    {
        Task ExecuteAsync(PayRegistrationFeeCourseCommand command);
    }

    public class PayRegistrationFeeCourseUseCase : IPayRegistrationFeeUseCase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;

        public PayRegistrationFeeCourseUseCase(ICourseRepository courseRepository, IStudentRepository studentRepository)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        }

        public async Task ExecuteAsync(PayRegistrationFeeCourseCommand command)
        {
            command.ValidateIfFailThrow();

            Course course = _courseRepository.GetByIdOrDefault(command.CourseId)
                ?? throw new CourseNotFoundException();

            Student student = _studentRepository.GetByIdOrDefault(command.StudentId)
                ?? throw new StudentNotFoundException();


            //PaymentRegistrationFee p =  course.EmitPayRegistrationFee(student, command.RegistrationFeePaymentRequest.Amount);
            
            
            
            if (command.RegistrationFeePaymentRequest.Amount < course.RegistrationFee)
                throw new PaymentAmountInsufficientException();

            
        }
    }
}
