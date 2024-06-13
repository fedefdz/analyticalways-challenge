using AcmeSchool.Domain.ValueObjects;

namespace AcmeSchool.Domain.Entities
{
    public class Student
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public DateTime BirthDate { get; private set; }

        public Student(string name, DateTime birthDate)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be empty.", nameof(name));
            }

            Name = name;
            BirthDate = birthDate;
        }

        public int GetAge()
        {
            return new DateOfBirth(BirthDate).GetAge();
        }
    }
}
