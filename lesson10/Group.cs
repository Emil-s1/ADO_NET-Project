namespace CollegeApp.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public override string ToString() => $"ID: {Id} | {Name}";
}
