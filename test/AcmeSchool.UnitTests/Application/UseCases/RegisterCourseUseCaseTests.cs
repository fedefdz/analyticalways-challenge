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
        private readonly Mock<ICourseRepository> _mockRepository;
        private readonly RegisterCourseUseCase _useCase;
        private readonly Fixture _fixture;

        public RegisterCourseUseCaseTests()
        {
            _mockRepository = new Mock<ICourseRepository>();
            _useCase = new RegisterCourseUseCase(_mockRepository.Object);
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(0)]
        public async Task Execute_With_CourseRegistrationFeeNoPositive_Throws_CourseInvalidDataException(decimal registrationFee)
        {
            // Arrange
            var startDate = DateTime.Now;
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, registrationFee, startDate, startDate.AddMonths(3));

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert            
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _mockRepository.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Execute_With_CourseStartDateDefault_Throws_CourseInvalidDataException()
        {
            // Arrange
            DateTime startDate = default;
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, 999, startDate, startDate.AddMonths(3));

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert            
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _mockRepository.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Execute_With_CourseEndDateDefault_Throws_CourseInvalidDataException()
        {
            // Arrange
            DateTime endDate = default;
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, 999, DateTime.Now, endDate);

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert            
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _mockRepository.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Execute_With_CourseStartDateGreaterThanEndDate_Throws_CourseInvalidDataException()
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
            _mockRepository.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Execute_With_CourseStartDatePreviousCurrentDate_Throws_CourseInvalidDataException()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(-1);
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, 999, startDate, startDate.AddMonths(3));

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert            
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _mockRepository.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Execute_With_CourseMeetsRequeriment_Then_AddsCourse()
        {
            // Arrange
            var startDate = DateTime.Now.Date.AddDays(1);
            var courseName = _fixture.Create<string>();
            var courseCommand = new RegisterCourseCommand(courseName, 999, startDate, startDate.AddMonths(3));

            _mockRepository.Setup(mock => mock.GetByNameOrDefaultAsync(courseCommand.Name)).ReturnsAsync((Course?)null);

            // Act
            await _useCase.ExecuteAsync(courseCommand);

            // Assert
            _mockRepository.Verify(mock => mock.AddAsync(It.Is<Course>(x =>
                x.Name == courseCommand.Name &&
                x.RegistrationFee == courseCommand.RegistrationFee &&
                x.StartDate == courseCommand.StartDate &&
                x.EndDate == courseCommand.EndDate)),
                Times.Once);
        }

        [Fact]
        public async Task Execute_With_CourseThatAlreadyExists_Throws_CourseAlreadyExistsException()
        {
            // Arrange
            var courseName = _fixture.Create<string>();
            var startDate = DateTime.Now.Date.AddDays(1);
            var courseCommand = new RegisterCourseCommand(courseName, 999, startDate, startDate.AddDays(15));

            var courseExistent =  new Course(courseName, 678, startDate.AddDays(-10), startDate);
            _mockRepository.Setup(repo => repo.GetByNameOrDefaultAsync(courseCommand.Name)).ReturnsAsync(courseExistent);

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert
            (await result.Should().ThrowAsync<CourseAlreadyExistsException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseAlreadyExists);
            _mockRepository.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Execute_With_CourseNameEmpty_Throws_CourseInvalidDataException(string courseName)
        {
            // Arrange
            var startDate = DateTime.Now;
            var courseCommand = new RegisterCourseCommand(courseName, 100, startDate, startDate.AddMonths(3));

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(courseCommand);

            // Assert
            (await result.Should().ThrowAsync<CourseInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.CourseInvalidData);
            _mockRepository.Verify(mock => mock.AddAsync(It.IsAny<Course>()), Times.Never);
        }
    }
}
