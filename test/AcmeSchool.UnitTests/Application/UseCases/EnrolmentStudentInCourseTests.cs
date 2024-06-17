using AcmeSchool.Application.UseCases.EnrollStudentInCourse;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using AcmeSchool.Domain.ValueObjects;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;

namespace AcmeSchool.UnitTests.Application.UseCases
{
    public class EnrollStudentInCourseUseCaseTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<ICourseRepository> _courseRepositoryMock = new Mock<ICourseRepository>();
        private readonly Mock<IStudentRepository> _studentRepositoryMock = new Mock<IStudentRepository>();
        
        public EnrollStudentInCourseUseCaseTests()
        {
            _fixture.Inject<IStudentRepository>(_studentRepositoryMock.Object);
            _fixture.Inject<ICourseRepository>(_courseRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenCourseDoesNotExist_ThrowsCourseNotFoundException()
        {
            // Arrange
            var command = _fixture.Create<EnrollStudentInCourseCommand>();
            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync((Course?)null);

            var useCase = _fixture.Create<EnrollStudentInCourseUseCase>();

            // Act
            Func<Task> act = async () => await useCase.ExecuteAsync(command);

            // Assert
            await act.Should().ThrowAsync<CourseNotFoundException>();
        }

        [Fact]
        public async Task ExecuteAsync_WhenStudentDoesNotExist_ThrowsStudentNotFoundException()
        {
            // Arrange
            var command = _fixture.Create<EnrollStudentInCourseCommand>();
            var course = CreateCourseWithId(command.CourseId);
            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync((Student?)null);

            var useCase = _fixture.Create<EnrollStudentInCourseUseCase>();

            // Act
            Func<Task> act = async () => await useCase.ExecuteAsync(command);

            // Assert
            await act.Should().ThrowAsync<StudentNotFoundException>();
        }

        [Fact]
        public async Task ExecuteAsync_WhenValidConditions_CompletesSuccessfully()
        {
            // Arrange
            var command = _fixture.Create<EnrollStudentInCourseCommand>();
            var student = CreateStudentWithId(command.StudentId);
            var course = CreateCourseWithId(command.CourseId);

            var registrationsFeePaid= new RegistrationFeePaid(Guid.NewGuid(), student.Id, course.RegistrationFee, DateTime.Now, PaymentMethod.DebitCard);
            SetProperty(course, nameof(Course.RegistrationFeePayments), new []{ registrationsFeePaid });   

            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);
            _courseRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);

            var useCase = _fixture.Create<EnrollStudentInCourseUseCase>();
            
            // Act
            await useCase.ExecuteAsync(command);

            // Assert
            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Course>(c => c.IsStudentEnrolled(student))), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenStudentIsAlreadyEnrolled_ThrowsStudentAlreadyEnrolledException()
        {
            // Arrange
            var command = _fixture.Create<EnrollStudentInCourseCommand>();
            var student = CreateStudentWithId(command.StudentId);
            var course = CreateCourseWithId(command.CourseId);
            SetProperty(course, nameof(Course.Enrollments), new[] { new Enrollment(student.Id, student.Name) });

            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);

            var useCase = _fixture.Create<EnrollStudentInCourseUseCase>();

            // Act
            Func<Task> act = async () => await useCase.ExecuteAsync(command);

            // Assert
            await act.Should().ThrowAsync<StudentAlreadyEnrolledException>();
        }

        [Fact]
        public async Task ExecuteAsync_WhenRegistrationFeeIsNotPaid_ThrowsRegistrationFeeNotPaidException()
        {
            // Arrange
            var command = _fixture.Create<EnrollStudentInCourseCommand>();
            var student = CreateStudentWithId(command.StudentId);
            var course = CreateCourseWithId(command.CourseId);

            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);

            var useCase = _fixture.Create<EnrollStudentInCourseUseCase>();

            // Act
            Func<Task> act = async () => await useCase.ExecuteAsync(command);

            // Assert
            await act.Should().ThrowAsync<StudentRegistrationFeeNotPaidException>();
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
            SetProperty(student, nameof(Student.Id), studentId);
            return student;
        }

        private void SetProperty<T>(object instance, string propertyName, T value)
        {
            var property = instance.GetType().GetProperty(propertyName);
            property!.SetValue(instance, value);
        }
    }
}
