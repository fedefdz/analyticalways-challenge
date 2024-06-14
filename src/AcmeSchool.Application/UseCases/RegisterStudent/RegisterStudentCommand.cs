using AcmeSchool.Application.UseCases.Common;
using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;

namespace AcmeSchool.Application.UseCases.RegisterStudent
{
    public record RegisterStudentCommand(string Name, DateTime BirthDate) : UseCaseCommand
    {
        public override void ValidateIfFailThrow()
        {
            var student = new Student(Name, BirthDate);
            
            var age = student.GetAge();
            if (age < RegisterStudentUseCase.MinimumAgeToBeAdult) throw new StudentAgeInsuffiicientException();
        }
    }
}
