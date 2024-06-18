using AcmeSchool.Application.UseCases.RegisterCourse;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace AcmeSchool.UnitTests.Application.UseCases
{
    public class RegisterCourseUseCaseTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly RegisterCourseUseCase _useCase;
        private readonly Fixture _fixture;

        public RegisterCourseUseCaseTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _useCase = new RegisterCourseUseCase(_courseRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(0)]
        public async Task Execute_WithCourseRegistrationFeeNoPositive_ThrowsCourseInvalidDataException(decimal registrationFee)
        {
            // Arrange
            var startDate = DateTime.Now;
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, registrationFee, startDate, startDate.AddMonths(3));

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert            
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _courseRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WithCourseStartDateDefault_ThrowsCourseInvalidDataException()
        {
            // Arrange
            DateTime startDate = default;
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, 999, startDate, startDate.AddMonths(3));

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert            
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _courseRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WithCourseEndDateDefault_ThrowsCourseInvalidDataException()
        {
            // Arrange
            DateTime endDate = default;
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, 999, DateTime.Now, endDate);

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert            
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _courseRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WithCourseStartDateGreaterThanEndDate_ThrowsCourseInvalidDataException()
        {
            // Arrange
            DateTime endDate = DateTime.Now.AddMonths(3);
            var startDate = endDate.AddDays(+1);
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, 999, startDate, endDate);

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert            
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _courseRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WithCourseStartDatePreviousCurrentDate_ThrowsCourseInvalidDataException()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(-1);
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, 999, startDate, startDate.AddMonths(3));

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert            
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _courseRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WithCourseMeetsRequeriment_AddsCourse()
        {
            // Arrange
            var startDate = DateTime.Now.Date.AddDays(1);
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, 999, startDate, startDate.AddMonths(3));

            _courseRepositoryMock.Setup(mock => mock.GetByNameOrDefaultAsync(courseCommand.Name)).ReturnsAsync((Course?)null);

            // Act
            await _useCase.ExecuteAsync(courseCommand);

            // Assert
            _courseRepositoryMock.Verify(mock => mock.AddAsync(It.Is<Course>(x =>
                x.Name == courseCommand.Name &&
                x.RegistrationFee == courseCommand.RegistrationFee &&
                x.StartDate == courseCommand.StartDate &&
                x.EndDate == courseCommand.EndDate)),
                Times.Once);
        }

        [Fact]
        public async Task Execute_WithCourseThatAlreadyExists_ThrowsCourseAlreadyExistsException()
        {
            // Arrange
            var courseName = _fixture.Create<string>();
            var startDate = DateTime.Now.Date.AddDays(1);
            var courseCommand = new RegisterCourseCommand(courseName, 999, startDate, startDate.AddDays(15));

            var courseExistent =  new Course(courseName, 678, startDate.AddDays(-10), startDate);
            _courseRepositoryMock.Setup(repo => repo.GetByNameOrDefaultAsync(courseCommand.Name)).ReturnsAsync(courseExistent);

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert
            (await result.Should().ThrowAsync<CourseAlreadyExistsException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseAlreadyExists);
            _courseRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Execute_WithCourseNameEmpty_ThrowsCourseInvalidDataException(string courseName)
        {
            // Arrange
            var startDate = DateTime.Now;
            var courseCommand = new RegisterCourseCommand(courseName, 100, startDate, startDate.AddMonths(3));

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _courseRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }
    }
}
