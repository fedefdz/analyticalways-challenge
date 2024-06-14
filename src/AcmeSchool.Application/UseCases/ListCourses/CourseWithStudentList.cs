using AcmeSchool.Domain.Entities;

namespace AcmeSchool.Application.UseCases.ListCourses
{
    public record CourseWithStudentList(string CourseName, DateTime StartDate, DateTime EndDate, IEnumerable<string> Students)
    {
        public static CourseWithStudentList FromCourse(Course course)
        {
            return new CourseWithStudentList(course.Name, course.StartDate, course.EndDate, course.Enrollments.Select(s => s.StudentName));
        }
    }
}