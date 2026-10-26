namespace CollegeApi.Models;

public sealed class StudentCreate
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int? GroupId { get; set; }
}
