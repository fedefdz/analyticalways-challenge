using AcmeSchool.Domain.Entities;

namespace AcmeSchool.Domain.Repositories
{
    public interface IStudentRepository
    {
        Task AddAsync(Student student);
        Task<Student?> GetByNameOrDefaultAsync(string name);
        Task<Student?> GetByIdOrDefaultAsync(Guid id);
    }
}
