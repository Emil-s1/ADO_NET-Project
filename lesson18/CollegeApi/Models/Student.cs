namespace CollegeApi.Models;

public sealed class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int? GroupId { get; set; }
}
