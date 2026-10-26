using Dapper;
using CollegeApi.Models;

namespace CollegeApi.Data;

public sealed class GroupRepository
{
    private readonly SqlConnectionFactory _factory;

    public GroupRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Group>> GetAll()
    {
        using var connection = _factory.Create();
        return await connection.QueryAsync<Group>(
            "SELECT Id, Name FROM Groups ORDER BY Id");
    }
}
