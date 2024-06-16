using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.ValueObjects;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FluentAssertions;

namespace AcmeSchool.UnitTests.Domain.Entities
{
    public class CourseTests
    {
        [Fact]
        public void EnrollStudent_WithNotEnrolledAndFeePaid_EnrollsSuccessfully()
        {
            // Arrange
            var student = new Student("John Doe", DateTime.Now.AddYears(-20));
            var course = CreateCourseWithPaidFee(student.Id);

            // Act
            course.EnrollStudent(student);

            // Assert
            course.Enrollments.Should().ContainSingle(e => e.StudentId == student.Id);
        }

        [Fact]
        public void EnrollStudent_WhenStudentAlreadyEnrolled_ThrowsStudentAlreadyEnrolledException()
        {
            // Arrange
            var student = new Student("John Doe", DateTime.Now.AddYears(-20));
            var course = CreateCourseWithPaidFee(student.Id);
            course.EnrollStudent(student); 

            // Act
            Action act = () => course.EnrollStudent(student);

            // Assert
            act.Should().Throw<StudentAlreadyEnrolledException>();
        }

        [Fact]
        public void EnrollStudent_WhenFeeNotPaid_ThrowsStudentRegistrationFeeNotPaidException()
        {
            // Arrange
            var student = new Student("John Doe", DateTime.Now.AddYears(-20));
            var course = new Course("Test Course", 100m, DateTime.Today, DateTime.Today.AddDays(30));

            // Act
            Action act = () => course.EnrollStudent(student);

            // Assert
            act.Should().Throw<StudentRegistrationFeeNotPaidException>();
        }

        [Fact]
        public void PayRegistrationFee_WithApprovedPayment_AddsPaymentSuccessfully()
        {
            // Arrange
            var course = new Course("Test Course", 100m, DateTime.Today, DateTime.Today.AddDays(30));
            var payment = new CourseRegitrationFeePayment(course.Id, Guid.NewGuid(), 100, PaymentMethod.CreditCard);
            payment.Approbe(Guid.NewGuid().ToString());

            // Act
            course.PayRegistrationFee(payment);

            // Assert
            course.RegistrationFeePayments.Should().ContainSingle(p => p.Amount == 100m);
        }

        [Fact]
        public void PayRegistrationFee_WithMismatchedCourseId_ThrowsOperationNotAllowedException()
        {
            // Arrange
            var course = new Course("Test Course", 100m, DateTime.Today, DateTime.Today.AddDays(30));
            var otherCourse = Guid.NewGuid();
            var payment = new CourseRegitrationFeePayment(otherCourse, Guid.NewGuid(), 100, PaymentMethod.CreditCard); ;

            // Act
            Action act = () => course.PayRegistrationFee(payment);

            // Assert
            act.Should().Throw<OperationNotAllowedException>().WithMessage("payment is not for this course");
        }

        [Fact]
        public void PayRegistrationFee_WithInsufficientAmount_ThrowsPaymentAmountInsufficientException()
        {
            // Arrange
            var course = new Course("Test Course", 100m, DateTime.Today, DateTime.Today.AddDays(30));
            var payment = new CourseRegitrationFeePayment(course.Id, Guid.NewGuid(), course.RegistrationFee-1, PaymentMethod.CreditCard);

            // Act
            Action act = () => course.PayRegistrationFee(payment);

            // Assert
            act.Should().Throw<PaymentAmountInsufficientException>();
        }

        [Fact]
        public void PayRegistrationFee_WithUnapprovedPayment_ThrowsOperationNotAllowedException()
        {
            // Arrange
            var course = new Course("Test Course", 100m, DateTime.Today, DateTime.Today.AddDays(30));
            var payment = new CourseRegitrationFeePayment(course.Id, Guid.NewGuid(), 100, PaymentMethod.CreditCard);

            // Act
            Action act = () => course.PayRegistrationFee(payment);

            // Assert
            act.Should().Throw<OperationNotAllowedException>().WithMessage("payment is not approved");
        }

        private Course CreateCourseWithPaidFee(Guid studentId)
        {
            var course = new Course("Test Course", 100m, DateTime.Today, DateTime.Today.AddDays(30));
            var payment = new ResgitrationFeePayment(Guid.NewGuid(), studentId, 100m, DateTime.Today, PaymentMethod.CreditCard);
            // Utilizando reflexión para agregar el pago a la lista privada, ya que no hay un método público disponible
            var paymentsField = course.GetType().GetProperty(nameof(Course.RegistrationFeePayments));
            var paymentsList = (List<ResgitrationFeePayment>)paymentsField!.GetValue(course)!;
            paymentsList!.Add(payment);
            paymentsField.SetValue(course, paymentsList);
            return course;
        }

    }

    public class CourseConstructorTests
    {
        [Theory, AutoMoqData]
        public void Constructor_WithValidParameters_SetsPropertiesCorrectly(
            string name,
            decimal registrationFee,
            DateTime startDate,
            DateTime endDate,
            IFixture fixture)
        {
            // Arrange
            endDate = startDate.AddDays(10); // Ensure endDate is after startDate for a valid scenario
            fixture.Customize(new AutoMoqCustomization());

            // Act
            var course = new Course(name, registrationFee, startDate, endDate);

            // Assert
            course.Name.Should().Be(name);
            course.RegistrationFee.Should().Be(registrationFee);
            course.StartDate.Should().Be(startDate);
            course.EndDate.Should().Be(endDate);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_WithInvalidName_ThrowsCourseInvalidDataException(string invalidName)
        {
            // Arrange
            var registrationFee = 100m;
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(30);

            // Act
            Action act = () => new Course(invalidName, registrationFee, startDate, endDate);

            // Assert
            act.Should().Throw<CourseInvalidDataException>()
                .WithMessage("*Name*could not be empty*");
        }

        [Theory, AutoMoqData]
        public void Constructor_WithNegativeRegistrationFee_ThrowsCourseInvalidDataException(
            string name,
            DateTime startDate,
            DateTime endDate,
            IFixture fixture)
        {
            // Arrange
            var invalidRegistrationFee = -100m; 
            fixture.Customize(new AutoMoqCustomization());

            // Act
            Action act = () => new Course(name, invalidRegistrationFee, startDate, endDate);

            // Assert
            act.Should().Throw<CourseInvalidDataException>()
                .WithMessage("*RegistrationFee*could not be negative or zero*");
        }

        
    }

    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}
