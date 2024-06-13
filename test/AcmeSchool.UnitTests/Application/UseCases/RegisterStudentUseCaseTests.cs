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
        private readonly Mock<IStudentRepository> _mockRepository;
        private readonly RegisterStudentUseCase _useCase;
        private readonly Fixture _fixture;

        public RegisterStudentUseCaseTests()
        {
            _mockRepository = new Mock<IStudentRepository>();
            _useCase = new RegisterStudentUseCase(_mockRepository.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public void Execute_With_StudentUnderMinimumAge_Throws_StudentAgeInsufficientException()
        {
            // Arrange
            var birthDate = DateTime.Now.AddYears(-RegisterStudentUseCase.MinimumAgeToBeAdult + 1); // Makes the student underage by one year
            var studentName = _fixture.Create<string>();
            var underageStudentCommand = new RegisterStudentCommand(studentName, birthDate);

            // Act
            Action result = () => _useCase.Execute(underageStudentCommand);

            // Assert            
            result.Should().Throw<StudentAgeInsuffcientException>().Which.ErrorCode.Should().Be((int)DomainErrorCodes.StudentAgeInsuffcient);
            _mockRepository.Verify(mock => mock.Add(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public void Execute_With_StudentMeetsMinimumAge_Then_AddsStudent()
        {
            // Arrange
            var birthDate = DateTime.Now.AddYears(-RegisterStudentUseCase.MinimumAgeToBeAdult); // Makes the student with adult age
            var studentName = _fixture.Create<string>();
            var adultStudentCommand = new RegisterStudentCommand(studentName, birthDate);

            _mockRepository.Setup(mock => mock.GetByNameOrDefault(adultStudentCommand.Name)).Returns((Student?)null);

            // Act
            _useCase.Execute(adultStudentCommand);

            // Assert
            _mockRepository.Verify(mock => mock.Add(It.Is<Student>(x => 
                x.Name == adultStudentCommand.Name &&
                x.BirthDate == adultStudentCommand.BirthDate)),
                Times.Once);
        }

        [Fact]
        public void Execute_With_StudentThatAlreadyExists_Throws_StudentAlreadyExistsException()
        {
            // Arrange
            var birthDate = DateTime.Now.AddYears(-RegisterStudentUseCase.MinimumAgeToBeAdult);
            var studentName = _fixture.Create<string>();
            var studentCommand = new RegisterStudentCommand(studentName, birthDate);

            _mockRepository.Setup(repo => repo.GetByNameOrDefault(studentCommand.Name)).Returns(new Student(studentCommand.Name, studentCommand.BirthDate));

            // Act
            Action result = () => _useCase.Execute(studentCommand);

            // Assert
            result.Should().Throw<StudentAlreadyExistsException>().Which.ErrorCode.Should().Be((int)DomainErrorCodes.StudentAlreadyExists);
            _mockRepository.Verify(mock => mock.Add(It.IsAny<Student>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]        
        [InlineData(" ")]
        public void Execute_With_StudentNameEmpty_Throws_StudentInvalidDataException(string studentName)
        {
            // Arrange
            var birthDate = DateTime.Now.AddYears(-RegisterStudentUseCase.MinimumAgeToBeAdult);
            var studentCommand = new RegisterStudentCommand(studentName, birthDate);

            // Act
            Action result = () => _useCase.Execute(studentCommand);

            // Assert
            result.Should().Throw<StudentInvalidDataException>().Which.ErrorCode.Should().Be((int)DomainErrorCodes.StudentInvalidData);
            _mockRepository.Verify(mock => mock.Add(It.IsAny<Student>()), Times.Never);
        }
    }
}
