using AcmeSchool.Domain.Entities;
using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.Repositories;
using AcmeSchool.Domain.Specifications;

namespace AcmeSchool.Application.UseCases.RegisterStudent
{
    public class RegisterStudentUseCase
    {
        public const int MinimumAgeToBeAdult = 18;

        private readonly IStudentRepository _studentRepository;
        private readonly ISpecification<Student> _ageSpecification;
        
        public RegisterStudentUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _ageSpecification = new StudentMinimumAgeSpecification(MinimumAgeToBeAdult);
        }

        public void Execute(RegisterStudentCommand command)
        {
            var student = new Student(command.Name, command.BirthDate);

            ValidateStudentIfFailThrow(student);

            _studentRepository.Add(student);
        }

        private void ValidateStudentIfFailThrow(Student student)
        {
            if (_studentRepository.GetByNameOrDefault(student.Name) != null)
                throw new StudentAlreadyExistsException();

            if (!_ageSpecification.IsSatisfiedBy(student))
                throw new StudentAgeInsuffcientException();
        }

    }
}
