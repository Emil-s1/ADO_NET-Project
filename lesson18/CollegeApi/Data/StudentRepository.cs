using Dapper;
using CollegeApi.Models;

namespace CollegeApi.Data;

public sealed class StudentRepository
{
    private readonly SqlConnectionFactory _factory;

    public StudentRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Student>> GetAll()
    {
        using var connection = _factory.Create();
        const string sql = "SELECT Id, FirstName, LastName, GroupId FROM Students ORDER BY Id";
        return await connection.QueryAsync<Student>(sql);
    }

    public async Task<Student?> GetById(int id)
    {
        using var connection = _factory.Create();
        const string sql = "SELECT Id, FirstName, LastName, GroupId FROM Students WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Student>(sql, new { Id = id });
    }

    public async Task<int> Create(StudentCreate model)
    {
        using var connection = _factory.Create();
        const string sql = @"
INSERT INTO Students (FirstName, LastName, GroupId)
OUTPUT INSERTED.Id
VALUES (@FirstName, @LastName, @GroupId)";
        return await connection.ExecuteScalarAsync<int>(sql, model);
    }

    public async Task<bool> Update(int id, StudentCreate model)
    {
        using var connection = _factory.Create();
        const string sql = @"
UPDATE Students
SET FirstName = @FirstName, LastName = @LastName, GroupId = @GroupId
WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new
        {
            Id = id,
            model.FirstName,
            model.LastName,
            model.GroupId
        }) > 0;
    }

    public async Task<bool> Delete(int id)
    {
        using var connection = _factory.Create();
        return await connection.ExecuteAsync(
            "DELETE FROM Students WHERE Id = @Id", new { Id = id }) > 0;
    }
}
