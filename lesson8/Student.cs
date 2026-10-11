namespace CollegeApp.Models;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? GroupId { get; set; }

    public override string ToString()
        => $"ID: {Id} | {LastName} {FirstName} | Group ID: {(GroupId?.ToString() ?? "не указана")}";
}
