using AcmeSchool.Domain.Entities;
using FluentAssertions;

namespace AcmeSchool.UnitTests.Domain.Entities
{
    public class StudentTests
    {
        [Fact]
        public void GetAge_WhenBirthdayHasPassedThisYear_ReturnsAge_ThatIs_DiffBetweenCurrentYearAndBirthYear()
        {
            // Arrange
            var birthDate = new DateTime(DateTime.Now.Year - 20, DateTime.Now.Month - 1, DateTime.Now.Day);
            var student = new Student("John Doe", birthDate);

            // Act
            var age = student.GetAge();

            // Assert
            age.Should().Be(20);
        }

        [Fact]
        public void GetAge_WhenBirthdayIsToday_ReturnsAge_ThatIs_DiffBetweenCurrentYearAndBirthYear()
        {
            // Arrange
            var birthDate = new DateTime(DateTime.Now.Year - 20, DateTime.Now.Month, DateTime.Now.Day);
            var student = new Student("John Doe", birthDate);

            // Act
            var age = student.GetAge();

            // Assert
            age.Should().Be(20);
        }

        [Fact]
        public void GetAge_WhenBirthdayHasNotPassedThisYear_ReturnsAge_ThatIs_DiffBetweenCurrentYearAndBirthYearLessOne()
        {
            // Arrange
            var birthDate = new DateTime(DateTime.Now.Year - 20, DateTime.Now.Month + 1, DateTime.Now.Day);
            var student = new Student("John Doe", birthDate);

            // Act
            var age = student.GetAge();

            // Assert
            age.Should().Be(19);
        }
    }
}
