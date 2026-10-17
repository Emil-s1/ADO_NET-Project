using CollegeApi.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CollegeApi.Data;

public sealed class GroupRepository(SqlConnection connection)
{
    public async Task<IEnumerable<Group>> GetAllAsync()
    {
        const string sql = "SELECT GroupId, GroupName FROM Groups ORDER BY GroupId";
        return await connection.QueryAsync<Group>(sql);
    }
}
