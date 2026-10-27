namespace CourseRegistration.Models;

public class Student
{
    public string Id { get; set; } = string.Empty;

    public HashSet<string> CompletedCourses { get; set; }
        = new();
}