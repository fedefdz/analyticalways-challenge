using AcmeSchool.Application.UseCases.ContractCourse;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using AcmeSchool.Domain.ValueObjects;
using AutoFixture.AutoMoq;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace AcmeSchool.UnitTests.Application.UseCases
{
    public class ContractCourseUseCaseTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<IStudentRepository> _studentRepositoryMock = new Mock<IStudentRepository>();
        private readonly Mock<ICourseRepository> _courseRepositoryMock = new Mock<ICourseRepository>();
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock = new Mock<IPaymentRepository>();

        public ContractCourseUseCaseTests()
        {
            _fixture.Inject<IStudentRepository>(_studentRepositoryMock.Object);
            _fixture.Inject<ICourseRepository>(_courseRepositoryMock.Object);
            _fixture.Inject<IPaymentRepository>(_paymentRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ValidCommand_ReturnsPendingPayment()
        {
            // Arrange
            var command = _fixture.Create<ContractCourseCommand>();
            var course =  CreateCourseWithId(command.CourseId);
            var student = CreateStudentWithId(command.StudentId);
            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);
            var useCase = _fixture.Create<ContractCourseUseCase>();

            // Act
            var result = await useCase.ExecuteAsync(command);

            // Assert
            _paymentRepositoryMock.Verify(repo => repo.AddCourseRegistrationFeePaymentAsync(It.IsAny<RegistrationFeePayment>()), Times.Once);
            result.PaymentId.Should().NotBeEmpty();
            result.CourseId.Should().Be(course.Id);
            result.StudentId.Should().Be(student.Id);
            result.Status.Should().Be(PaymentStatus.Pending);
            result.Amount.Should().Be(course.RegistrationFee);
            result.PaymentDate.Should().BeNull();
            result.ApprovationCode.Should().BeNull();
        }

        [Fact]
        public async Task ExecuteAsync_CourseNotFound_ThrowsCourseNotFoundException()
        {
            // Arrange
            var command = _fixture.Create<ContractCourseCommand>();
            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync((Course?)null);
            var useCase = _fixture.Create<ContractCourseUseCase>();

            // Act
            Func<Task> act = async () => await useCase.ExecuteAsync(command);

            // Assert
            await act.Should().ThrowAsync<CourseNotFoundException>();
        }

        [Fact]
        public async Task ExecuteAsync_StudentNotFound_ThrowsStudentNotFoundException()
        {
            // Arrange
            var command = _fixture.Create<ContractCourseCommand>();
            var course = CreateCourseWithId(command.CourseId);
            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync((Student?)null);
            var useCase = _fixture.Create<ContractCourseUseCase>();

            // Act
            Func<Task> act = async () => await useCase.ExecuteAsync(command);

            // Assert
            await act.Should().ThrowAsync<StudentNotFoundException>();
        }

        [Fact]
        public async Task ExecuteAsync_StudentAlreadyEnrolled_ThrowsOperationNotAllowedException()
        {
            // Arrange
            var command = _fixture.Create<ContractCourseCommand>();
            var course = CreateCourseWithId(command.CourseId);
            var student = CreateStudentWithId(command.StudentId);
            
            var approvedPayment = new RegistrationFeePayment(course.Id, student.Id, course.RegistrationFee, PaymentMethod.BankTransfer);
            approvedPayment.Approve(_fixture.Create<string>());
            course.PayRegistrationFee(approvedPayment);
            course.EnrollStudent(student);

            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);
            
            var useCase = _fixture.Create<ContractCourseUseCase>();

            // Act
            Func<Task> act = async () => await useCase.ExecuteAsync(command);

            // Assert
            await act.Should().ThrowAsync<OperationNotAllowedException>();
        }

        [Fact]
        public async Task ExecuteAsync_RegistrationFeeAlreadyPaid_ThrowsOperationNotAllowedException()
        {
            // Arrange
            var command = _fixture.Create<ContractCourseCommand>();
            var course = CreateCourseWithId(command.CourseId);
            var student = CreateStudentWithId(command.StudentId);

            var approvedPayment = new RegistrationFeePayment(course.Id, student.Id, course.RegistrationFee, PaymentMethod.BankTransfer);
            approvedPayment.Approve(_fixture.Create<string>());
            course.PayRegistrationFee(approvedPayment);

            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);
            
            var useCase = _fixture.Create<ContractCourseUseCase>();

            // Act & Assert
            await Assert.ThrowsAsync<OperationNotAllowedException>(() => useCase.ExecuteAsync(command));
        }

        private Course CreateCourseWithId(Guid courseId)
        {
            var today = DateTime.Today;
            var registrationFee = Math.Abs(_fixture.Create<decimal>()) + 0.01m;
            var course = new Course(_fixture.Create<string>(), registrationFee, today.AddMonths(1), today.AddMonths(2));

            var property = course.GetType().GetProperty(nameof(Course.Id));
            property!.SetValue(course, courseId);
            return course;
        }

        private Student CreateStudentWithId(Guid studentId)
        {
            var student = _fixture.Create<Student>();

            var property = student.GetType().GetProperty(nameof(Student.Id));
            property!.SetValue(student, studentId);
            return student;
        }
    }
}
