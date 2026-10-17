namespace CollegeApi.Models;

public sealed class Student
{
    public int StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? GroupId { get; set; }
}
