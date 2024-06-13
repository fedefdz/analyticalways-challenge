namespace AcmeSchool.Application.UseCases.EnrollStudentInCourse
{
    public class EnrollStudentInCourseUseCase
    {
        public void Execute(EnrollStudentInCourseCommand command)
        {
            command.ValidateIfFailThrow();
        }
    }
}
