using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;

namespace AcmeSchool.Application.UseCases.RegisterStudent
{
    public class RegisterStudentUseCase
    {
        public const int MinimumAgeToBeAdult = 18;

        private readonly IStudentRepository _studentRepository;
        
        public RegisterStudentUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));            
        }

        public void Execute(RegisterStudentCommand command)
        {
            ValidateCommandIfFailThrow(command);

            var student = new Student(command.Name, command.BirthDate);
            
            _studentRepository.Add(student);
        }

        private void ValidateCommandIfFailThrow(RegisterStudentCommand command)
        {
            command.ValidateIfFailThrow();
            
            if (_studentRepository.GetByNameOrDefault(command.Name) != null)
                throw new StudentAlreadyExistsException();
        }

    }
}
