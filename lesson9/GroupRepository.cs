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
        const string sql = "SELECT Id, Name FROM dbo.Groups ORDER BY Name;";

        using var connection = new SqlConnection(_connectionString);
        return connection.Query<Group>(sql).AsList();
    }

    public Group? GetById(int id)
    {
        const string sql = "SELECT Id, Name FROM dbo.Groups WHERE Id = @Id;";

        using var connection = new SqlConnection(_connectionString);
        return connection.QueryFirstOrDefault<Group>(sql, new { Id = id });
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

        const string sql = "UPDATE dbo.Groups SET Name = @Name WHERE Id = @Id;";

        using var connection = new SqlConnection(_connectionString);
        return connection.Execute(sql, new
        {
            group.Id,
            Name = group.Name.Trim()
        }) > 0;
    }

    public bool Delete(int id)
    {
        const string sql = "DELETE FROM dbo.Groups WHERE Id = @Id;";

        using var connection = new SqlConnection(_connectionString);
        return connection.Execute(sql, new { Id = id }) > 0;
    }
}
