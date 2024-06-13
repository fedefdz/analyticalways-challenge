using AcmeSchool.Domain.Entities;

namespace AcmeSchool.Domain.Specifications
{
    public class StudentMinimumAgeSpecification : ISpecification<Student>
    {
        private readonly int _minimumAge;

        public StudentMinimumAgeSpecification(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        public bool IsSatisfiedBy(Student student)
        {
            return student.GetAge() >= _minimumAge;
        }
    }
}
