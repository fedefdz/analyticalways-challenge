using AcmeSchool.Domain.Entities;
using FluentAssertions;

namespace AcmeSchool.UnitTests.Domain.Entities
{
    public class StudentTests
    {
        [Fact]
        public void GetAge_When_BirthdayHasPassedThisYear_Returns_CorrectAge_ThatIs_DiffBetweenCurrentYearAndBirthYear()
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
        public void GetAge_When_BirthdayIsToday_Returns_CorrectAge_ThatIs_DiffBetweenCurrentYearAndBirthYear()
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
        public void GetAge_When_BirthdayHasNotPassedThisYear_Returns_CorrectAge_ThatIs_DiffBetweenCurrentYearAndBirthYearLessOne()
        {
            // Arrange
            var birthDate = new DateTime(DateTime.Now.Year - 20, DateTime.Now.Month + 1, DateTime.Now.Day);
            var student = new Student("John Doe", birthDate);

            // Act
            var age = student.GetAge();

            // Assert
            age.Should().Be(19); // Since the birthday has not passed yet this year, the age should be one less.
        }
    }
}
