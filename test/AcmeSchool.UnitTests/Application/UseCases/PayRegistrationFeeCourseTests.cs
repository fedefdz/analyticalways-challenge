using AcmeSchool.Application.Services.PaymentGateway;
using AcmeSchool.Application.UseCases.PayRegistrationFeeCourse;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using AcmeSchool.Domain.ValueObjects;
using AcmeSchool.UnitTests.Common;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;

namespace AcmeSchool.UnitTests.Application.UseCases
{
    public class PayRegistrationFeeCourseUseCaseTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<ICourseRepository> _courseRepositoryMock = new Mock<ICourseRepository>();
        private readonly Mock<IStudentRepository> _studentRepositoryMock = new Mock<IStudentRepository>();
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock = new Mock<IPaymentRepository>();
        private readonly Mock<IPaymentGatewayService> _paymentGatewayMock = new Mock<IPaymentGatewayService>();        

        public PayRegistrationFeeCourseUseCaseTests()
        {
            _fixture.Inject<IStudentRepository>(_studentRepositoryMock.Object);
            _fixture.Inject<ICourseRepository>(_courseRepositoryMock.Object);
            _fixture.Inject<IPaymentRepository>(_paymentRepositoryMock.Object);
            _fixture.Inject<IPaymentGatewayService>(_paymentGatewayMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenPaymentAmountIsLessThanCourseRegistrationFee_ThrowsPaymentAmountInsufficientException()
        {
            // Arrange
            var command = _fixture.Create<PayRegistrationFeeCourseCommand>();
            command = command with { RegistrationFeePaymentRequest = command.RegistrationFeePaymentRequest with { Amount = 500m } };

            Course course = CreateCourseWithId(command.CourseId);
            Setter.SetProperty(course, nameof(Course.RegistrationFee), 1000m);
            Student student = CreateStudentWithId(command.StudentId);
            RegistrationFeePayment registrationFeePayment = CreateRegistrationFeePaymentWithId(command.RegistrationFeePaymentRequest.PaymentId);
            
            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);
            _paymentRepositoryMock.Setup(repo => repo.GetCourseRegistrationFeePaymentByIdOrDefaultAsync(command.RegistrationFeePaymentRequest.PaymentId)).ReturnsAsync(registrationFeePayment);

            var useCase = _fixture.Create<PayRegistrationFeeCourseUseCase>();

            // Act
            Func<Task> act = async () => await useCase.ExecuteAsync(command);

            // Assert
            await  act.Should().ThrowAsync<PaymentAmountInsufficientException>();
        }

        [Fact]
        public async Task ExecuteAsync_WhenPaymentIsNotPendingBeforeToPay_ThrowsOperationNotAllowedException()
        {
            // Arrange
            var command = _fixture.Create<PayRegistrationFeeCourseCommand>();
            Course course = CreateCourseWithId(command.CourseId);
            Setter.SetProperty(course, nameof(Course.RegistrationFee), command.RegistrationFeePaymentRequest.Amount);
            Student student = CreateStudentWithId(command.StudentId);
            RegistrationFeePayment registrationFeePayment = CreateRegistrationFeePaymentWithId(command.RegistrationFeePaymentRequest.PaymentId);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.Status), PaymentStatus.Approved);

            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);
            _paymentRepositoryMock.Setup(repo => repo.GetCourseRegistrationFeePaymentByIdOrDefaultAsync(command.RegistrationFeePaymentRequest.PaymentId)).ReturnsAsync(registrationFeePayment);

            var useCase = _fixture.Create<PayRegistrationFeeCourseUseCase>();

            // Act
            Func<Task> act = async () => await useCase.ExecuteAsync(command);

            // Assert
            await act.Should().ThrowAsync<OperationNotAllowedException>();
        }

        [Fact]
        public async Task ExecuteAsync_WhenPaymentAmountMatchesCourseRegistrationFee_ThenProcessPayment()
        {
            // Arrange
            var command = _fixture.Create<PayRegistrationFeeCourseCommand>();
            command = command with { RegistrationFeePaymentRequest = command.RegistrationFeePaymentRequest with { Amount = 500m } };
            Course course = CreateCourseWithId(command.CourseId);
            Setter.SetProperty(course, nameof(Course.RegistrationFee), command.RegistrationFeePaymentRequest.Amount);
            Student student = CreateStudentWithId(command.StudentId);
            RegistrationFeePayment registrationFeePayment = CreateRegistrationFeePaymentWithId(command.RegistrationFeePaymentRequest.PaymentId);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.CourseId), course.Id);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.StudentId), student.Id);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.Status), PaymentStatus.Pending);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.Amount), command.RegistrationFeePaymentRequest.Amount);

            PaymentResult paymentResult = CreatePaymentResultWithId(command.RegistrationFeePaymentRequest.PaymentId);

            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);
            _paymentRepositoryMock.Setup(repo => repo.GetCourseRegistrationFeePaymentByIdOrDefaultAsync(command.RegistrationFeePaymentRequest.PaymentId)).ReturnsAsync(registrationFeePayment);
            _paymentGatewayMock.Setup(service => service.ProcessPaymentAsync(command.RegistrationFeePaymentRequest)).ReturnsAsync(paymentResult);
            _paymentRepositoryMock.Setup(repo => repo.UpdateCourseRegistrationFeePaymentAsync(registrationFeePayment)).Returns(Task.CompletedTask);

            var useCase = _fixture.Create<PayRegistrationFeeCourseUseCase>();

            // Act
            RegistrationFeePayment result = await useCase.ExecuteAsync(command);

            // Assert
            _paymentRepositoryMock.Verify(repo => repo.UpdateCourseRegistrationFeePaymentAsync(registrationFeePayment), Times.Once);            
        }


        [Fact]
        public async Task ExecuteAsync_WhenPaymentResultIsApproved_ThenConfirmRegistrationFeeaAyment()
        {
            // Arrange
            var command = _fixture.Create<PayRegistrationFeeCourseCommand>();
            command = command with { RegistrationFeePaymentRequest = command.RegistrationFeePaymentRequest with { Amount = 500m } };
            Course course = CreateCourseWithId(command.CourseId);
            Setter.SetProperty(course, nameof(Course.RegistrationFee), command.RegistrationFeePaymentRequest.Amount);
            Student student = CreateStudentWithId(command.StudentId);
            RegistrationFeePayment registrationFeePayment = CreateRegistrationFeePaymentWithId(command.RegistrationFeePaymentRequest.PaymentId);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.CourseId), course.Id);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.StudentId), student.Id);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.Status), PaymentStatus.Pending);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.Amount), command.RegistrationFeePaymentRequest.Amount);

            PaymentResult paymentResult = CreatePaymentResultWithId(command.RegistrationFeePaymentRequest.PaymentId);
            Setter.SetProperty(paymentResult, nameof(PaymentResult.Success), true);
            Setter.SetProperty(paymentResult, nameof(PaymentResult.ResultCode), PaymentResultCodes.Success);
            Setter.SetProperty(paymentResult, nameof(PaymentResult.ApprovationCode), _fixture.Create<string>());

            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);
            _paymentRepositoryMock.Setup(repo => repo.GetCourseRegistrationFeePaymentByIdOrDefaultAsync(command.RegistrationFeePaymentRequest.PaymentId)).ReturnsAsync(registrationFeePayment);
            _paymentGatewayMock.Setup(service => service.ProcessPaymentAsync(command.RegistrationFeePaymentRequest)).ReturnsAsync(paymentResult);
            _paymentRepositoryMock.Setup(repo => repo.UpdateCourseRegistrationFeePaymentAsync(registrationFeePayment)).Returns(Task.CompletedTask);
            _courseRepositoryMock.Setup(repo => repo.UpdateAsync(course)).Returns(Task.CompletedTask);
            
            var useCase = _fixture.Create<PayRegistrationFeeCourseUseCase>();

            // Act
            RegistrationFeePayment result = await useCase.ExecuteAsync(command);

            // Assert
            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(course), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenPaymentResultIsNotApproved_ThenShouldNotConfirmRegistrationFeePAyment()
        {
            // Arrange
            var command = _fixture.Create<PayRegistrationFeeCourseCommand>();       
            command = command with { RegistrationFeePaymentRequest = command.RegistrationFeePaymentRequest with { Amount = 500m } };
            Course course = CreateCourseWithId(command.CourseId);
            Setter.SetProperty(course, nameof(Course.RegistrationFee), command.RegistrationFeePaymentRequest.Amount);
            Student student = CreateStudentWithId(command.StudentId);
            RegistrationFeePayment registrationFeePayment = CreateRegistrationFeePaymentWithId(command.RegistrationFeePaymentRequest.PaymentId);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.CourseId), course.Id);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.StudentId), student.Id);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.Status), PaymentStatus.Pending);
            Setter.SetProperty(registrationFeePayment, nameof(RegistrationFeePayment.Amount), command.RegistrationFeePaymentRequest.Amount);

            PaymentResult paymentResult = CreatePaymentResultWithId(command.RegistrationFeePaymentRequest.PaymentId);
            if (paymentResult.Success)
            {
                Setter.SetProperty(paymentResult, nameof(PaymentResult.Success), false);
                Setter.SetProperty(paymentResult, nameof(PaymentResult.ResultCode), PaymentResultCodes.InsufficientFunds);
            }

            _courseRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.CourseId)).ReturnsAsync(course);
            _studentRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(command.StudentId)).ReturnsAsync(student);
            _paymentRepositoryMock.Setup(repo => repo.GetCourseRegistrationFeePaymentByIdOrDefaultAsync(command.RegistrationFeePaymentRequest.PaymentId)).ReturnsAsync(registrationFeePayment);
            _paymentGatewayMock.Setup(service => service.ProcessPaymentAsync(command.RegistrationFeePaymentRequest)).ReturnsAsync(paymentResult);
            _paymentRepositoryMock.Setup(repo => repo.UpdateCourseRegistrationFeePaymentAsync(registrationFeePayment)).Returns(Task.CompletedTask);
            _courseRepositoryMock.Setup(repo => repo.UpdateAsync(course)).Returns(Task.CompletedTask);

            var useCase = _fixture.Create<PayRegistrationFeeCourseUseCase>();

            // Act
            RegistrationFeePayment result = await useCase.ExecuteAsync(command);

            // Assert
            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(course), Times.Never);
        }

        private PaymentResult CreatePaymentResultWithId(Guid paymentId)
        {
            var resultCodes = new[] { PaymentResultCodes.Success, PaymentResultCodes.InsufficientFunds, PaymentResultCodes.UnknownError };
            var randomResultCode = resultCodes[ Math.Abs(_fixture.Create<int>()) % resultCodes.Length];
            var paymentResult = new PaymentResult(paymentId,
                resultCode: randomResultCode,                
                operationNumber: _fixture.Create<string>(),
                approvationCode: randomResultCode == PaymentResultCodes.Success ? _fixture.Create<string>() : null);

            return paymentResult;            
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
            Setter.SetProperty(student, nameof(Student.Id), studentId);
            return student;
        }

        private RegistrationFeePayment CreateRegistrationFeePaymentWithId(Guid paymentId)
        {
            var reagistrationFeePayment = _fixture.Create<RegistrationFeePayment>();
            Setter.SetProperty(reagistrationFeePayment, nameof(RegistrationFeePayment.PaymentId), paymentId);
            return reagistrationFeePayment;
        }        
    }
}
