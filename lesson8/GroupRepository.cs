using Microsoft.Data.SqlClient;
using CollegeApp.Models;

namespace CollegeApp.Data;

public class GroupRepository(string connectionString)
{
    private readonly string _connectionString =
        connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public List<Group> GetAll()
    {
        const string sql = "SELECT Id, Name FROM dbo.Groups ORDER BY Name;";
        var result = new List<Group>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();

        while (reader.Read())
            result.Add(Map(reader));

        return result;
    }

    public Group? GetById(int id)
    {
        const string sql = "SELECT Id, Name FROM dbo.Groups WHERE Id = @Id;";
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        connection.Open();
        using var reader = command.ExecuteReader();

        return reader.Read() ? Map(reader) : null;
    }

    public int Add(Group group)
    {
        ArgumentNullException.ThrowIfNull(group);
        const string sql = """
            INSERT INTO dbo.Groups (Name)
            OUTPUT INSERTED.Id
            VALUES (@Name);
            """;

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", group.Name.Trim());
        connection.Open();

        group.Id = Convert.ToInt32(command.ExecuteScalar());
        return group.Id;
    }

    public bool Update(Group group)
    {
        ArgumentNullException.ThrowIfNull(group);
        const string sql = "UPDATE dbo.Groups SET Name = @Name WHERE Id = @Id;";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", group.Name.Trim());
        command.Parameters.AddWithValue("@Id", group.Id);
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "DELETE FROM dbo.Groups WHERE Id = @Id;", connection);
        command.Parameters.AddWithValue("@Id", id);
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    private static Group Map(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name"))
    };
}
