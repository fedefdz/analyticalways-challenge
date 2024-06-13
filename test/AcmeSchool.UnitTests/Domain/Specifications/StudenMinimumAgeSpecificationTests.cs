using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Specifications;
using FluentAssertions;

namespace AcmeSchool.UnitTests.Domain.Specifications
{
    public class StudenMinimumAgeSpecificationTests
    {
        [Fact]
        public void IsSatisfiedBy_When_StudentAgeIsLessThanMinimumAge_Returns_False()
        {
            // Arrange
            var student = new Student("John Doe", DateTime.Now.AddYears(-17));
            var specification = new StudentMinimumAgeSpecification(18);

            // Act
            var result = specification.IsSatisfiedBy(student);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsSatisfiedBy_When_StudentAgeIsEqualToMinimumAge_Returns_True()
        {
            // Arrange
            var student = new Student("John Doe", DateTime.Now.AddYears(-18));
            var specification = new StudentMinimumAgeSpecification(18);

            // Act
            var result = specification.IsSatisfiedBy(student);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsSatisfiedBy_When_StudentAgeIsGreaterThanMinimumAge_Returns_True()
        {
            // Arrange
            var student = new Student("John Doe", DateTime.Now.AddYears(-19));
            var specification = new StudentMinimumAgeSpecification(18);

            // Act
            var result = specification.IsSatisfiedBy(student);

            // Assert
            result.Should().BeTrue();
        }
    }
}
