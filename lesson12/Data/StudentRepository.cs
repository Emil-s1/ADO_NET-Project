using CollegeApi.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CollegeApi.Data;

public sealed class StudentRepository(SqlConnection connection)
{
    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        const string sql = "SELECT StudentId, FirstName, LastName, GroupId FROM Students";
        return await connection.QueryAsync<Student>(sql);
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        const string sql = "SELECT StudentId, FirstName, LastName, GroupId FROM Students WHERE StudentId = @Id";
        return await connection.QuerySingleOrDefaultAsync<Student>(sql, new { Id = id });
    }
}
