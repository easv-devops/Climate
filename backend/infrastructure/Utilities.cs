namespace infrastructure;
public class Utilities
{
    private static readonly string _connectionString = Environment.GetEnvironmentVariable("pgconn")!;
    

    public static readonly string
        MySqlConnectionString = _connectionString;
}