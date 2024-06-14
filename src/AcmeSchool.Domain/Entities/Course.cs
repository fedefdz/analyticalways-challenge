using AcmeSchool.Domain.Exceptions;
using AcmeSchool.Domain.ValueObjects;

namespace AcmeSchool.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal RegistrationFee { get; private set; }
        public DateTime StartDate{ get; private set; }
        public DateTime EndDate { get; private set; }
        public IList<ResgitrationFeePayment> RegistrationFeePayments { get; private set; }
        public IList<Enrollment> Enrollments { get; private set; }


        public Course(string name, decimal registrationFee, DateTime startDate, DateTime endDate)
        {
            Id = Guid.NewGuid();
            Name = name;
            RegistrationFee = registrationFee;
            StartDate = startDate;
            EndDate = endDate;

            Enrollments = [];
            RegistrationFeePayments = [];

            Validate(this);
        }

        public void EnrollStudent(Student student)
        {            
            if (Enrollments.Any(e => e.StudentId == student.Id)) throw new StudentAlreadyEnrolledException(Name);

            if (!StudentHasRegistrationFeePaid(student.Id)) throw new StudentRegistrationFeeNotPaidException();

            var enrollment = new Enrollment(student.Id, student.Name);
            Enrollments.Add(enrollment);
        }

        private bool StudentHasRegistrationFeePaid(Guid id)
        {
            return RegistrationFeePayments.Any(p => p.StudentId == id);
        }

        public void PayRegistrationFee(CourseRegitrationFeePayment regitrationFeePayment)
        {
            if (regitrationFeePayment.CourseId != Id) throw new OperationNotAllowedException("payment is not for this course");
            if (regitrationFeePayment.Amount < RegistrationFee) throw new PaymentAmountInsufficientException();
            if (regitrationFeePayment.Status != PaymentStatus.Approved) throw new OperationNotAllowedException("payment is not approved");

            var payment = new ResgitrationFeePayment(regitrationFeePayment.PaymentId, regitrationFeePayment.StudentId, regitrationFeePayment.Amount, regitrationFeePayment.PaymentDate!.Value, regitrationFeePayment.PaymentMethod);
            RegistrationFeePayments.Add(payment);
        }

        public bool IsStudentEnrolled(Student student)
        {
            return Enrollments.Any(e => e.StudentId == student.Id);
        }

        public bool HasStudentRegitrationFeePaid(Student student)
        {
            return RegistrationFeePayments.Any(p => p.StudentId == student.Id);
        }

        public static void Validate(Course course)
        {
            if (string.IsNullOrWhiteSpace(course.Name)) throw new CourseInvalidDataException(nameof(course.Name), "could not be empty");
            if (course.RegistrationFee <= 0) throw new CourseInvalidDataException(nameof(course.RegistrationFee), "could not be negative or zero");
            if (course.StartDate == default) throw new CourseInvalidDataException(nameof(course.StartDate), "could not be default");
            if (course.EndDate == default) throw new CourseInvalidDataException(nameof(course.EndDate), "could not be default");
            if (course.StartDate > course.EndDate) throw new CourseInvalidDataException(nameof(course.StartDate), $"could not be greater than {nameof(course.EndDate)}");
        }        
    }
}
