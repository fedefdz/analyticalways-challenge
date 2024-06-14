namespace AcmeSchool.Domain.ValueObjects
{
    public class Enrollment
    {
        public Guid StudentId { get; private set; }
        public string StudentName { get; private set; }
        public DateTime EnrollmentDate { get; private set; }
        public Enrollment(Guid studentId, string studentName)
        {
            StudentId = studentId;
            StudentName = studentName;
            EnrollmentDate = DateTime.Now;
        }
    }
}
