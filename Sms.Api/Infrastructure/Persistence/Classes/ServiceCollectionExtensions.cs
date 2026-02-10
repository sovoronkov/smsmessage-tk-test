using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Persistence.Main;
using Persistence.Repositories;
using Repositories.Interfaces;



namespace Persistence.Classes;

static public class ServiceCollectionExtensions
{
    const string DB_CONNECTION_STRING = "DB_CONNECTION_STRING";
    const string CONNECTION_STRING_NOT_FOUND = "Connection string not found";
    public static void AddDataBase(this IServiceCollection services, IConfiguration conf)
    {
        var connectionString = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(DB_CONNECTION_STRING))
            ? conf.GetConnectionString("ConnectionStringDB")
            : Environment.GetEnvironmentVariable(DB_CONNECTION_STRING);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
            throw new Exception(CONNECTION_STRING_NOT_FOUND);
        }

        var s = Regex.Replace(connectionString, @"PASSWORD=[^;]*;", "PASSWORD=***;", RegexOptions.IgnoreCase);
        var s1 = Regex.Replace(s, @"password=[^;]*;", "password=***", RegexOptions.IgnoreCase);
        Console.WriteLine($"--- ConnectionString is {s}");

        services.AddDbContext<AppDbContext>(options => options.UseOracle(connectionString,
            opt => opt.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)), ServiceLifetime.Scoped);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
