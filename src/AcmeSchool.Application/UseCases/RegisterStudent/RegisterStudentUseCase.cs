using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using System.Runtime.CompilerServices;

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

        public async Task ExecuteAsync(RegisterStudentCommand command)
        {
            await ValidateCommandIfFailThrow(command);

            var student = new Student(command.Name, command.BirthDate);
            
            await _studentRepository.AddAsync(student);
        }

        private async Task ValidateCommandIfFailThrow(RegisterStudentCommand command)
        {
            command.ValidateIfFailThrow();
            
            if (await _studentRepository.GetByNameOrDefaultAsync(command.Name) != null)
                throw new StudentAlreadyExistsException();
        }

    }
}
