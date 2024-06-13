using AcmeSchool.Domain.Entities;

namespace AcmeSchool.Domain.Repositories
{
    public interface IStudentRepository
    {
        void Add(Student student);
        Student? GetByNameOrDefault(string name);
    }
}
