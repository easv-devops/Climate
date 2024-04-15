namespace infrastructure;
public class Utilities
{
    private static readonly string _connectionString = Environment.GetEnvironmentVariable("dbconn")!;
    

    public static readonly string
        MySqlConnectionString = _connectionString;
}