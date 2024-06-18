using AcmeSchool.Application.UseCases.ListCourses;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace AcmeSchool.UnitTests.Application.UseCases
{
    public class ListCoursesUsesCasesTests
    {
        private readonly Mock<ICourseRepository> _mockCourseRepository = new Mock<ICourseRepository>();
        private readonly ListCoursesUsesCases _useCase;

        public ListCoursesUsesCasesTests()
        {
            _useCase = new ListCoursesUsesCases(_mockCourseRepository.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsCorrectCoursesWithinDateRange()
        {
            // Arrange
            var fromDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 12, 31);
            var courses = new List<Course>
            {
                new("Test Course 1", 100m, fromDate, endDate.AddDays(-10)),
                new("Test Course 2", 50m, fromDate.AddDays(5), endDate),
            };

            _mockCourseRepository.Setup(repo => repo.GetAllBetweenRangeDatesAsync(fromDate, endDate))
                .ReturnsAsync(courses);

            var command = new ListCoursesCommand(FromDate: fromDate, EndDate: endDate);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(courses.Count);
            result.Should().SatisfyRespectively(
                first =>
                {
                    first.CourseName.Should().Be(courses[0].Name);
                    first.StartDate.Should().Be(courses[0].StartDate);
                    first.EndDate.Should().Be(courses[0].EndDate);
                    first.Students.Should().BeEquivalentTo(courses[0].Enrollments.Select(x => x.StudentName));
                },
                second =>
                {
                    second.CourseName.Should().Be(courses[1].Name);
                    second.StartDate.Should().Be(courses[1].StartDate);
                    second.EndDate.Should().Be(courses[1].EndDate);
                    second.Students.Should().BeEquivalentTo(courses[1].Enrollments.Select(x => x.StudentName));
                });
        }

        [Fact]
        public async Task ExecuteAsync_WhenFromDateIsGreaterThanEndDate_ThrowsOperationNotAllowedException()
        {
            // Arrange
            var command = new ListCoursesCommand(
                FromDate: new DateTime(2023, 12, 31),
                EndDate: new DateTime(2023, 1, 1)
            );

            // Act
            Func<Task> act = async () => await _useCase.ExecuteAsync(command);

            // Act & Assert
            await act.Should().ThrowAsync<OperationNotAllowedException>();
        }
    }
}
