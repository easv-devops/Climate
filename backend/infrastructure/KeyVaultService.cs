using api.helpers;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace infrastructure;

public static class KeyVaultService
{
    public static string GetSecret(string secretName)
    {
        var kvUri = "https://climatekv.vault.azure.net/";
        SecretClientOptions options = new SecretClientOptions()
        {
            Retry =
            {
                Delay= TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(16),
                MaxRetries = 5,
                Mode = RetryMode.Exponential
            }
        };

        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential(),options);

        try
        {
            KeyVaultSecret secret = client.GetSecret(secretName);
            System.Threading.Thread.Sleep(5000);
            return secret.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred while retrieving connection string from Azure Key Vault: {e.Message}");
            return null;
        }
        
    }

    public static string GetToken()
    {
        string JwtKey = Environment.GetEnvironmentVariable(EnvVarKeys.JwtKey.ToString());
        if (string.IsNullOrEmpty(JwtKey))
        {
            JwtKey = GetSecret(EnvVarKeys.JwtKey.ToString());
        }

        return JwtKey;

    }
    
    public static string GetDbConn()
    {
        var connectionString = Environment.GetEnvironmentVariable(EnvVarKeys.dbconn.ToString());
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = GetSecret(EnvVarKeys.dbconn.ToString());
        }

        return connectionString;
    }
    
    public static string GetMailPassword()
    {
        var mailPassword = Environment.GetEnvironmentVariable(EnvVarKeys.MailPassword.ToString());
        if (string.IsNullOrEmpty(mailPassword))
        {
            mailPassword = GetSecret(EnvVarKeys.MailPassword.ToString());
        }

        return mailPassword;
    }
    
    public static string GetMqttToken()
    {
        var mqttToken = Environment.GetEnvironmentVariable(EnvVarKeys.MqttToken.ToString());
        if (string.IsNullOrEmpty(mqttToken))
        {
            mqttToken = GetSecret(EnvVarKeys.MqttToken.ToString());
        }

        return mqttToken;
    }
}
