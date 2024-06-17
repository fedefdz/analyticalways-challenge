using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using AcmeSchool.Domain.ValueObjects;
using System.Xml.Linq;

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

            Course course = await GetCourseOrThrow(command.CourseId);
            Student student = await GetStudentOrThrow(command.StudentId);
            
            course.EnrollStudent(student);

            await _courseRepository.UpdateAsync(course);
        }

        private async Task<Course> GetCourseOrThrow(Guid courseId)
            => await _courseRepository.GetByIdOrDefaultAsync(courseId) ?? throw new CourseNotFoundException();

        private async Task<Student> GetStudentOrThrow(Guid studentId)
            => await _studentRepository.GetByIdOrDefaultAsync(studentId) ?? throw new StudentNotFoundException();
    }
}
