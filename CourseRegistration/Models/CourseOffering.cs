namespace CourseRegistration.Models;

public class CourseOffering
{
    public string CourseId { get; set; } = "CS101";

    public string RequiredCourse { get; set; } = "INTRO101";

    public int Capacity { get; set; }

    public HashSet<string> EnrolledStudents { get; set; }
        = new();

    public Queue<string> Waitlist { get; set; }
        = new();
}