namespace AcmeSchool.Application.UseCases.EnrollStudentInCourse
{
    public interface IEnrollStudentInCourseUseCase
    {
        Task ExecuteAsync(EnrollStudentInCourseCommand command);
    }

    public class EnrollStudentInCourseUseCase : IEnrollStudentInCourseUseCase
    {
        public async Task ExecuteAsync(EnrollStudentInCourseCommand command)
        {
            command.ValidateIfFailThrow();
        }
    }
}
