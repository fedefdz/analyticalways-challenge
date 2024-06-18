using AcmeSchool.Application.UseCases.RegisterStudent;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace AcmeSchool.UnitTests.Application.UseCases.RegisterStudent
{
    public class RegisterStudentUseCaseTests
    {
        private readonly Mock<IStudentRepository> _studentRepositoryMock;
        private readonly RegisterStudentUseCase _useCase;
        private readonly Fixture _fixture;

        public RegisterStudentUseCaseTests()
        {
            _studentRepositoryMock = new Mock<IStudentRepository>();
            _useCase = new RegisterStudentUseCase(_studentRepositoryMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Execute_WithStudentUnderMinimumAge_ThrowsStudentAgeInsufficientException()
        {
            // Arrange
            var birthDate = DateTime.Now.AddYears(-RegisterStudentUseCase.MinimumAgeToBeAdult + 1); // Makes the student underage by one year
            var studentName = _fixture.Create<string>();
            var underageStudentCommand = new RegisterStudentCommand(studentName, birthDate);

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(underageStudentCommand);

            // Assert            
            (await result.Should().ThrowAsync<StudentAgeInsuffiicientException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.StudentAgeInsuffcient);
            _studentRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WithStudentMeetsMinimumAge_ThenAddsStudent()
        {
            // Arrange
            var birthDate = DateTime.Now.AddYears(-RegisterStudentUseCase.MinimumAgeToBeAdult); // Makes the student with adult age
            var studentName = _fixture.Create<string>();
            var adultStudentCommand = new RegisterStudentCommand(studentName, birthDate);

            _studentRepositoryMock.Setup(mock => mock.GetByNameOrDefaultAsync(adultStudentCommand.Name)).ReturnsAsync((Student?)null);

            // Act
            await _useCase.ExecuteAsync(adultStudentCommand);

            // Assert
            _studentRepositoryMock.Verify(mock => mock.AddAsync(It.Is<Student>(x => 
                x.Name == adultStudentCommand.Name &&
                x.BirthDate == adultStudentCommand.BirthDate)),
                Times.Once);
        }

        [Fact]
        public async Task Execute_WithStudentThatAlreadyExists_ThrowsStudentAlreadyExistsException()
        {
            // Arrange
            var birthDate = DateTime.Now.AddYears(-RegisterStudentUseCase.MinimumAgeToBeAdult);
            var studentName = _fixture.Create<string>();
            var studentCommand = new RegisterStudentCommand(studentName, birthDate);

            _studentRepositoryMock.Setup(repo => repo.GetByNameOrDefaultAsync(studentCommand.Name)).ReturnsAsync(new Student(studentCommand.Name, studentCommand.BirthDate));

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(studentCommand);

            // Assert
            (await result.Should().ThrowAsync<StudentAlreadyExistsException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.StudentAlreadyExists);
            _studentRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Student>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]        
        [InlineData(" ")]
        public async Task Execute_WithStudentNameEmpty_ThrowsStudentInvalidDataException(string studentName)
        {
            // Arrange
            var birthDate = DateTime.Now.AddYears(-RegisterStudentUseCase.MinimumAgeToBeAdult);
            var studentCommand = new RegisterStudentCommand(studentName, birthDate);

            // Act
            Func<Task> result = async () => await _useCase.ExecuteAsync(studentCommand);

            // Assert
            (await result.Should().ThrowAsync<StudentInvalidDataException>()).Which.ErrorCode.Should().Be((int)DomainErrorCodes.StudentInvalidData);
            _studentRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Student>()), Times.Never);
        }
    }
}
