using api.helpers;

namespace infrastructure;
public class Utilities
{
    public static readonly string
        MySqlConnectionString = Environment.GetEnvironmentVariable(EnvVarKeys.dbconn.ToString());
}