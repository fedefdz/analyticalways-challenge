using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Repositories;

namespace AcmeSchool.Application.UseCases.ListCourses
{
    public class ListCoursesUsesCases
    {
        private readonly ICourseRepository _courseRepository;

        public ListCoursesUsesCases(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        }

        public async Task<IEnumerable<CourseWithStudentList>> ExecuteAsync(ListCoursesCommand command)
        {
            IEnumerable<Course> courses = await _courseRepository.GetAllBetweenRangeDatesAsync(command.FromDate, command.EndDate);
            var result = courses.Select(CourseWithStudentList.FromCourse);

            return result;
        }
    }
}
