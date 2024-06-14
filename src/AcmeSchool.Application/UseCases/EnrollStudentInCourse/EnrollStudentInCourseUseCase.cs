using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;

namespace AcmeSchool.Application.UseCases.EnrollStudentInCourse
{
    public interface IEnrollStudentInCourseUseCase
    {
        Task ExecuteAsync(EnrollStudentInCourseCommand command);
    }

    public class EnrollStudentInCourseUseCase : IEnrollStudentInCourseUseCase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;


        public EnrollStudentInCourseUseCase(ICourseRepository courseRepository, IStudentRepository studentRepository)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        }

        public async Task ExecuteAsync(EnrollStudentInCourseCommand command)
        {
            command.ValidateIfFailThrow();

            Course course = await _courseRepository.GetByIdOrDefaultAsync(command.CourseId)
                ?? throw new CourseNotFoundException();

            Student student = await _studentRepository.GetByIdOrDefaultAsync(command.StudentId)
                ?? throw new StudentNotFoundException();

            course.EnrollStudent(student);

            await _courseRepository.UpdateAsync(course);
        }
    }
}
