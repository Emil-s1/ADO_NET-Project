using CollegeApi.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CollegeApi.Data;

public sealed class GroupRepository(SqlConnection connection)
{
    public async Task<IEnumerable<Group>> GetAllAsync()
    {
        const string sql = "SELECT GroupId, GroupName FROM Groups";
        return await connection.QueryAsync<Group>(sql);
    }

    public async Task<Group?> GetByIdAsync(int id)
    {
        const string sql = "SELECT GroupId, GroupName FROM Groups WHERE GroupId = @Id";
        return await connection.QuerySingleOrDefaultAsync<Group>(sql, new { Id = id });
    }
}
