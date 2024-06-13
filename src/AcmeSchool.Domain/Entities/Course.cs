using AcmeSchool.Domain.Exceptions;

namespace AcmeSchool.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal RegistrationFee { get; private set; }
        public DateTime StartDate{ get; private set; }
        public DateTime EndDate { get; private set; }

        public Course(string name, decimal registrationFee, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new CourseInvalidDataException(nameof(name), "could not be empty");
            if (registrationFee <= 0) throw new CourseInvalidDataException(nameof(registrationFee), "could not be negative or zero");
            if (startDate == default) throw new CourseInvalidDataException(nameof(startDate), "could not be default");
            if (endDate == default) throw new CourseInvalidDataException(nameof(endDate), "could not be default");
            if (startDate > endDate) throw new CourseInvalidDataException(nameof(startDate), $"could not be greater than {nameof(endDate)}");

            Id = Guid.NewGuid();
            Name = name;
            RegistrationFee = registrationFee;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
