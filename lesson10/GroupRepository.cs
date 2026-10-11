using Dapper;
using Microsoft.Data.SqlClient;
using CollegeApp.Models;

namespace CollegeApp.Data;

public class GroupRepository(string connectionString)
{
    private readonly string _connectionString =
        connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public IEnumerable<Group> GetAll()
    {
        using var connection = new SqlConnection(_connectionString);
        return connection.Query<Group>(
            "SELECT Id, Name FROM dbo.Groups ORDER BY Name;").AsList();
    }

    public Group? GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        return connection.QueryFirstOrDefault<Group>(
            "SELECT Id, Name FROM dbo.Groups WHERE Id = @Id;", new { Id = id });
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
        int id = connection.QuerySingle<int>(sql, new { Name = group.Name.Trim() });
        group.Id = id;
        return id;
    }

    public bool Update(Group group)
    {
        ArgumentNullException.ThrowIfNull(group);

        using var connection = new SqlConnection(_connectionString);
        return connection.Execute(
            "UPDATE dbo.Groups SET Name = @Name WHERE Id = @Id;",
            new { group.Id, Name = group.Name.Trim() }) > 0;
    }

    public bool Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        return connection.Execute(
            "DELETE FROM dbo.Groups WHERE Id = @Id;", new { Id = id }) > 0;
    }
}
