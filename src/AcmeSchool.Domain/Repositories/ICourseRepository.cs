using AcmeSchool.Domain.Entities;

namespace AcmeSchool.Domain.Repositories
{
    public interface ICourseRepository
    {
        void Add(Course course);
        Course? GetByNameOrDefault(string name);
        Course? GetByIdOrDefault(Guid id);
    }
}
