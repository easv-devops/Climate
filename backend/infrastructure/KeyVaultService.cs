using api.helpers;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace infrastructure;

public static class KeyVaultService
{
    public static async Task<string> GetSecret(string secretName)
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
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);
            return secret.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred while retrieving connection string from Azure Key Vault: {e.Message}");
            return null;
        }
        
    }

    public static async Task<string> GetToken()
    {
        string JwtKey = Environment.GetEnvironmentVariable(EnvVarKeys.JwtKey.ToString());
        if (string.IsNullOrEmpty(JwtKey))
        {
            JwtKey = await GetSecret(EnvVarKeys.JwtKey.ToString());
        }

        return JwtKey;

    }
    
    public static async Task<string>  GetDbConn()
    {
        var connectionString = Environment.GetEnvironmentVariable(EnvVarKeys.dbconn.ToString());
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = await GetSecret(EnvVarKeys.dbconn.ToString());
        }

        return connectionString;
    }
    
    public static async Task<string>  GetMailPassword()
    {
        var mailPassword = Environment.GetEnvironmentVariable(EnvVarKeys.MailPassword.ToString());
        if (string.IsNullOrEmpty(mailPassword))
        {
            mailPassword = await GetSecret(EnvVarKeys.MailPassword.ToString());
        }

        return mailPassword;
    }
    
    public static async Task<string>  GetMqttToken()
    {
        var mqttToken = Environment.GetEnvironmentVariable(EnvVarKeys.MqttToken.ToString());
        if (string.IsNullOrEmpty(mqttToken))
        {
            mqttToken = await GetSecret(EnvVarKeys.MqttToken.ToString());
        }

        return mqttToken;
    }
}
