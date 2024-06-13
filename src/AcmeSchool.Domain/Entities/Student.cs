using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.ValueObjects;
using System.Runtime.CompilerServices;

namespace AcmeSchool.Domain.Entities
{
    public class Student
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public DateTime BirthDate { get; private set; }

        public Student(string name, DateTime birthDate)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new StudentInvalidDataException(nameof(name), "could not be empty");
            if (birthDate == default) throw new StudentInvalidDataException(nameof(birthDate), "could not be default"); 

            Id = Guid.NewGuid();
            Name = name;
            BirthDate = birthDate;
        }

        public int GetAge()
        {
            return new DateOfBirth(BirthDate).GetAge();
        }
    }
}
