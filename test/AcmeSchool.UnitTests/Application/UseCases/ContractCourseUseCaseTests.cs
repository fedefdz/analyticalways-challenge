using AcmeSchool.Application.UseCases.ContractCourse;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using AcmeSchool.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace AcmeSchool.UnitTests.Application.UseCases
{
    public class ContractCourseUseCaseTests
    {
        private readonly Mock<IStudentRepository> _studentRepositoryMock = new();
        private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock = new();
        private readonly ContractCourseUseCase _useCase;

        public ContractCourseUseCaseTests()
        {
            _useCase = new ContractCourseUseCase(_studentRepositoryMock.Object, _courseRepositoryMock.Object, _paymentRepositoryMock.Object);
        }

        [Fact]
        public async Task ExcecuteAsync_WithValidCommand_ReturnsPayment()
        {
            // Arrange
            var course = new Course("Test Course", 100m, DateTime.Today, DateTime.Today.AddDays(30));
            var student = new Student("Test Student", DateTime.Now.AddYears(-20));
            var command = new ContractCourseCommand(course.Id, student.Id, PaymentMethod: PaymentMethod.CreditCard);


            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);
            _paymentRepositoryMock.Setup(repo => repo.AddCourseRegistrationFeePayementAsync(It.IsAny<CourseRegitrationFeePayment>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExcecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.CourseId.Should().Be(command.CourseId);
            result.StudentId.Should().Be(command.StudentId);
            result.Amount.Should().Be(course.RegistrationFee);
        }

        [Fact]
        public async Task ExcecuteAsync_WithNonexistentCourse_ThrowsCourseNotFoundException()
        {
            // Arrange
            var command = new ContractCourseCommand(CourseId: Guid.NewGuid(), StudentId: Guid.NewGuid(), PaymentMethod: PaymentMethod.CreditCard);

            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync((Course?)null);

            // Act
            Func<Task> act = async () => await _useCase.ExcecuteAsync(command);

            // Assert
            await act.Should().ThrowAsync<CourseNotFoundException>();
        }
    }
}
