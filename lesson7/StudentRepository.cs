using Microsoft.Data.SqlClient;
using CollegeApp.Models;

namespace CollegeApp.Repositories;

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

        var students = new List<Student>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            students.Add(MapStudent(reader));
        }

        return students;
    }

    public Student? GetById(int id)
    {
        const string sql = """
            SELECT Id, FirstName, LastName, GroupId
            FROM dbo.Students
            WHERE Id = @Id;
            """;

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        connection.Open();

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapStudent(reader) : null;
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
        AddStudentParameters(command, student);
        connection.Open();

        int id = Convert.ToInt32(command.ExecuteScalar());
        student.Id = id;
        return id;
    }

    public bool Update(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        const string sql = """
            UPDATE dbo.Students
            SET FirstName = @FirstName,
                LastName = @LastName,
                GroupId = @GroupId
            WHERE Id = @Id;
            """;

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        AddStudentParameters(command, student);
        command.Parameters.AddWithValue("@Id", student.Id);
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        const string sql = "DELETE FROM dbo.Students WHERE Id = @Id;";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    private static void AddStudentParameters(SqlCommand command, Student student)
    {
        command.Parameters.AddWithValue("@FirstName", student.FirstName.Trim());
        command.Parameters.AddWithValue("@LastName", student.LastName.Trim());
        command.Parameters.AddWithValue(
            "@GroupId", student.GroupId.HasValue ? student.GroupId.Value : DBNull.Value);
    }

    private static Student MapStudent(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        LastName = reader.GetString(reader.GetOrdinal("LastName")),
        GroupId = reader.IsDBNull(reader.GetOrdinal("GroupId"))
            ? null
            : reader.GetInt32(reader.GetOrdinal("GroupId"))
    };
}
