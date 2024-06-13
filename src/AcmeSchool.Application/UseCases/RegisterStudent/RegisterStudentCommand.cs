using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.ValueObjects;

namespace AcmeSchool.Application.UseCases.RegisterStudent
{
    public record RegisterStudentCommand(string Name, DateTime BirthDate) 
    {
        public void ValidateIfFailThrow()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new StudentInvalidDataException(nameof(Name));
            if (BirthDate == default) throw new StudentInvalidDataException(nameof(BirthDate));

            var age = new DateOfBirth(BirthDate).GetAge();
            if (age < RegisterStudentUseCase.MinimumAgeToBeAdult) throw new StudentAgeInsuffcientException();
        }
    }
}
