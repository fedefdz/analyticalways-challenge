using AcmeSchool.Application.UseCases.EnrollStudentInCourse;
using AcmeSchool.Application.UseCases.PayRegistrationFeeCourse;
using AcmeSchool.Application.UseCases.RegisterStudent;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;

namespace AcmeSchool.Application.UseCases.ContractCourse
{
    public interface IContractCourseUseCase
    {
        public Task ExcecuteAsync(ContractCourseCommand command);
    }

    public class ContractCourseUseCase : IContractCourseUseCase
    { 
        private readonly IPayRegistrationFeeUseCase _payRegistrationFeeUseCase;
        private readonly IEnrollStudentInCourseUseCase _enrollStudentInCourseUseCase;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;

        public ContractCourseUseCase(
            IPayRegistrationFeeUseCase payRegistrationFeeUseCase, IEnrollStudentInCourseUseCase enrollStudentInCourseUseCase,
            IStudentRepository studentRepository, ICourseRepository courseRepository)
        {
            _payRegistrationFeeUseCase = payRegistrationFeeUseCase ?? throw new ArgumentNullException(nameof(payRegistrationFeeUseCase));
            _enrollStudentInCourseUseCase = enrollStudentInCourseUseCase ?? throw new ArgumentNullException(nameof(enrollStudentInCourseUseCase));
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        }

        public async Task ExcecuteAsync(ContractCourseCommand command)
        {
            command.ValidateIfFailThrow();

            

            await _payRegistrationFeeUseCase.ExecuteAsync(new PayRegistrationFeeCourseCommand(command.CourseId, command.StudentId, command.RegistrationFeePaymentRequest));

            await _enrollStudentInCourseUseCase.ExecuteAsync(new EnrollStudentInCourseCommand(command.CourseId, command.StudentId));
        }

        
    }


}
