using Microsoft.Data.SqlClient;
using CollegeApp.Models;

namespace CollegeApp.Data;

public class StudentRepository(string connectionString)
{
    private readonly string _connectionString =
        connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public List<Student> GetAll()
    {
        const string sql = """
            SELECT Id, FirstName, LastName, GroupId
            FROM dbo.Students
            ORDER BY LastName, FirstName;
            """;

        var result = new List<Student>();
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();

        while (reader.Read())
            result.Add(Map(reader));

        return result;
    }

    public Student? GetById(int id)
    {
        const string sql = """
            SELECT Id, FirstName, LastName, GroupId
            FROM dbo.Students WHERE Id = @Id;
            """;

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        connection.Open();
        using var reader = command.ExecuteReader();

        return reader.Read() ? Map(reader) : null;
    }

    public int Add(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        const string sql = """
            INSERT INTO dbo.Students (FirstName, LastName, GroupId)
            OUTPUT INSERTED.Id
            VALUES (@FirstName, @LastName, @GroupId);
            """;

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        AddParameters(command, student);
        connection.Open();

        student.Id = Convert.ToInt32(command.ExecuteScalar());
        return student.Id;
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
        using var command = new SqlCommand(sql, connection);
        AddParameters(command, student);
        command.Parameters.AddWithValue("@Id", student.Id);
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "DELETE FROM dbo.Students WHERE Id = @Id;", connection);
        command.Parameters.AddWithValue("@Id", id);
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    private static void AddParameters(SqlCommand command, Student student)
    {
        command.Parameters.AddWithValue("@FirstName", student.FirstName.Trim());
        command.Parameters.AddWithValue("@LastName", student.LastName.Trim());
        command.Parameters.AddWithValue("@GroupId",
            student.GroupId.HasValue ? student.GroupId.Value : DBNull.Value);
    }

    private static Student Map(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        LastName = reader.GetString(reader.GetOrdinal("LastName")),
        GroupId = reader.IsDBNull(reader.GetOrdinal("GroupId"))
            ? null
            : reader.GetInt32(reader.GetOrdinal("GroupId"))
    };
}
