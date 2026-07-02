using FinAdvisor.Web.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace FinAdvisor.Web.Services;

public class SqliteConnectionFactory
{
    private readonly DatabaseOptions _options;

    public SqliteConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    public SqliteConnection Create()
    {
        var connection = new SqliteConnection(_options.ConnectionString);
        connection.Open();
        return connection;
    }
}
