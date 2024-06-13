namespace AcmeSchool.Domain.ValueObjects
{
    public class DateOfBirth
    {
        public DateTime Value { get; private set; }

        public DateOfBirth(DateTime value)
        {
            if (value == default)
            {
                throw new ArgumentException("Birth date cannot be empty.", nameof(value));
            }

            Value = value;
        }

        public int GetAge()
        {
            var age = DateTime.Now.Year - Value.Year;
            if (DateTime.Now.DayOfYear < Value.DayOfYear)
                age -= 1;

            return age;
        }

        public static implicit operator DateOfBirth(DateTime value) => new DateOfBirth(value);
    }
}
