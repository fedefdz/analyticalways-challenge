using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;

namespace AcmeSchool.Application.UseCases.RegisterCourse
{
    public class RegisterCourseUseCase
    {
        private readonly ICourseRepository _courseRepository;

        public RegisterCourseUseCase(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        }

        public void Execute(RegisterCourseCommand command)
        {
            ValidateCommandIfFailThrow(command);

            var course = new Course(command.Name, command.RegistrationFee, command.StartDate, command.EndDate);

            _courseRepository.Add(course);
        }

        private void ValidateCommandIfFailThrow(RegisterCourseCommand command)
        {
            command.ValidateIfFailThrow();

            if (_courseRepository.GetByNameOrDefault(command.Name) != null)
                throw new CourseAlreadyExistsException();
        }
    }
}
