using Microsoft.Data.SqlClient;

namespace CollegeApi.Data;

public sealed class SqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection Create() =>
        new(_configuration.GetConnectionString("CollegeDB"));
}
