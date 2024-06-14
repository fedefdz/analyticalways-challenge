using AcmeSchool.Domain.Entities;

namespace AcmeSchool.Domain.Repositories
{
    public interface ICourseRepository
    {
        Task AddAsync(Course course);
        Task<Course?> GetByNameOrDefaultAsync(string name);
        Task<Course?> GetByIdOrDefaultAsync(Guid id);
        Task UpdateAsync(Course course);
    }
}
