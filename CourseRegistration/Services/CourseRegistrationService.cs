using CourseRegistration.Enums;
using CourseRegistration.Models;

namespace CourseRegistration.Services;

public class CourseRegistrationService
{
    private readonly object _lock = new();

    private readonly CourseOffering _course;

    private readonly Dictionary<string, Student> _students;

    public CourseRegistrationService(
        CourseOffering course,
        Dictionary<string, Student> students)
    {
        _course = course;
        _students = students;
    }

    public EnrollmentResult Enroll(string studentId)
    {
        lock (_lock)
        {
            if (!_students.ContainsKey(studentId))
                return EnrollmentResult.StudentNotFound;

            var student = _students[studentId];

            if (!student.CompletedCourses.Contains(_course.RequiredCourse))
                return EnrollmentResult.PrerequisiteNotMet;

            if (_course.EnrolledStudents.Contains(studentId))
                return EnrollmentResult.AlreadyEnrolled;

            if (_course.Waitlist.Contains(studentId))
                return EnrollmentResult.AddedToWaitlist;

            if (_course.EnrolledStudents.Count < _course.Capacity)
            {
                _course.EnrolledStudents.Add(studentId);
                return EnrollmentResult.Success;
            }

            _course.Waitlist.Enqueue(studentId);

            return EnrollmentResult.AddedToWaitlist;
        }
    }

    public EnrollmentResult Drop(string studentId)
    {
        lock (_lock)
        {
            if (!_course.EnrolledStudents.Remove(studentId))
                return EnrollmentResult.NotEnrolled;

            PromoteWaitlistedStudent();

            return EnrollmentResult.Dropped;
        }
    }

    private void PromoteWaitlistedStudent()
    {
        while (_course.Waitlist.Count > 0)
        {
            var studentId = _course.Waitlist.Dequeue();

            if (!_students.ContainsKey(studentId))
                continue;

            var student = _students[studentId];

            if (student.CompletedCourses.Contains(_course.RequiredCourse))
            {
                _course.EnrolledStudents.Add(studentId);
                break;
            }
        }
    }
}