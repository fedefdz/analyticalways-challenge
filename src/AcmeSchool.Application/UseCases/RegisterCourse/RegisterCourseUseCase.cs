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

        public async Task ExecuteAsync(RegisterCourseCommand command)
        {
            await ValidateCommandIfFailThrow(command);

            var course = new Course(command.Name, command.RegistrationFee, command.StartDate, command.EndDate);

            await _courseRepository.AddAsync(course);
        }

        private async Task ValidateCommandIfFailThrow(RegisterCourseCommand command)
        {
            command.ValidateIfFailThrow();

            if (await _courseRepository.GetByNameOrDefaultAsync(command.Name) != null)
                throw new CourseAlreadyExistsException();
        }
    }
}
