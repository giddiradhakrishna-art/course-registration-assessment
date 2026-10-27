using CourseRegistration.Enums;
using CourseRegistration.Models;
using CourseRegistration.Services;

namespace CourseRegistration.Tests;

public class CourseRegistrationTests
{
    private static Dictionary<string, Student> CreateStudents()
    {
        return new()
        {
            {
                "S1",
                new Student
                {
                    Id = "S1",
                    CompletedCourses = new() { "INTRO101" }
                }
            },
            {
                "S2",
                new Student
                {
                    Id = "S2",
                    CompletedCourses = new() { "INTRO101" }
                }
            },
            {
                "S3",
                new Student
                {
                    Id = "S3",
                    CompletedCourses = new() { "INTRO101" }
                }
            },
            {
                "S4",
                new Student
                {
                    Id = "S4"
                }
            }
        };
    }

    [Fact]
    public void Enroll_WhenCapacityAvailable_ShouldSucceed()
    {
        var course = new CourseOffering { Capacity = 2 };
        var service =
            new CourseRegistrationService(course, CreateStudents());

        var result = service.Enroll("S1");

        Assert.Equal(EnrollmentResult.Success, result);
    }

    [Fact]
    public void FullCourse_ShouldAddToWaitlist()
    {
        var course = new CourseOffering { Capacity = 1 };

        var service =
            new CourseRegistrationService(course, CreateStudents());

        service.Enroll("S1");

        var result = service.Enroll("S2");

        Assert.Equal(
            EnrollmentResult.AddedToWaitlist,
            result);
    }

    [Fact]
    public void DuplicateEnrollment_ShouldBePrevented()
    {
        var course = new CourseOffering { Capacity = 2 };

        var service =
            new CourseRegistrationService(course, CreateStudents());

        service.Enroll("S1");

        var result = service.Enroll("S1");

        Assert.Equal(
            EnrollmentResult.AlreadyEnrolled,
            result);
    }

    [Fact]
    public void PrerequisiteFailure_ShouldBeReturned()
    {
        var course = new CourseOffering { Capacity = 2 };

        var service =
            new CourseRegistrationService(course, CreateStudents());

        var result = service.Enroll("S4");

        Assert.Equal(
            EnrollmentResult.PrerequisiteNotMet,
            result);
    }

    [Fact]
    public void UnknownStudent_ShouldReturnNotFound()
    {
        var course = new CourseOffering { Capacity = 2 };

        var service =
            new CourseRegistrationService(course, CreateStudents());

        var result = service.Enroll("XYZ");

        Assert.Equal(
            EnrollmentResult.StudentNotFound,
            result);
    }

    [Fact]
    public void FIFOWaitlistPromotion_ShouldWork()
    {
        var course = new CourseOffering { Capacity = 1 };

        var service =
            new CourseRegistrationService(course, CreateStudents());

        service.Enroll("S1");

        service.Enroll("S2");

        service.Enroll("S3");

        service.Drop("S1");

        Assert.Contains(
            "S2",
            course.EnrolledStudents);
    }

    [Fact]
    public void InvalidDrop_ShouldReturn_NotEnrolled()
    {
        var course = new CourseOffering { Capacity = 1 };

        var service =
            new CourseRegistrationService(course, CreateStudents());

        var result = service.Drop("S1");

        Assert.Equal(
            EnrollmentResult.NotEnrolled,
            result);
    }

    [Fact]
    public void DuplicateDrop_ShouldReturn_NotEnrolled()
    {
        var course = new CourseOffering { Capacity = 1 };

        var service =
            new CourseRegistrationService(course, CreateStudents());

        service.Enroll("S1");
        service.Drop("S1");

        var result = service.Drop("S1");

        Assert.Equal(
            EnrollmentResult.NotEnrolled,
            result);
    }
}