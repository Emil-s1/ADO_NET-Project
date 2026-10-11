using Dapper;
using Microsoft.Data.SqlClient;
using CollegeApp.Models;

namespace CollegeApp.Data;

public class StudentRepository(string connectionString)
{
    private readonly string _connectionString =
        connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public IEnumerable<Student> GetAll()
    {
        const string sql = """
            SELECT Id, FirstName, LastName, GroupId
            FROM dbo.Students
            ORDER BY LastName, FirstName;
            """;

        using var connection = new SqlConnection(_connectionString);
        return connection.Query<Student>(sql).AsList();
    }

    public Student? GetById(int id)
    {
        const string sql = """
            SELECT Id, FirstName, LastName, GroupId
            FROM dbo.Students WHERE Id = @Id;
            """;

        using var connection = new SqlConnection(_connectionString);
        return connection.QueryFirstOrDefault<Student>(sql, new { Id = id });
    }

    public int CreateStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        const string sql = """
            INSERT INTO dbo.Students (FirstName, LastName, GroupId)
            OUTPUT INSERTED.Id
            VALUES (@FirstName, @LastName, @GroupId);
            """;

        using var connection = new SqlConnection(_connectionString);
        int id = connection.QuerySingle<int>(sql, new
        {
            FirstName = student.FirstName.Trim(),
            LastName = student.LastName.Trim(),
            student.GroupId
        });

        student.Id = id;
        return id;
    }

    public bool Update(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        const string sql = """
            UPDATE dbo.Students
            SET FirstName = @FirstName, LastName = @LastName, GroupId = @GroupId
            WHERE Id = @Id;
            """;

        using var connection = new SqlConnection(_connectionString);
        return connection.Execute(sql, new
        {
            student.Id,
            FirstName = student.FirstName.Trim(),
            LastName = student.LastName.Trim(),
            student.GroupId
        }) > 0;
    }

    public bool Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        return connection.Execute(
            "DELETE FROM dbo.Students WHERE Id = @Id;", new { Id = id }) > 0;
    }
}
