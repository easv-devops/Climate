using api.helpers;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault.Keys;

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
    
    public static async Task<string> GetDbConn()
    {
        // In local development we all have an Environment Variable called dbconn
        var connectionString = Environment.GetEnvironmentVariable(EnvVarKeys.dbconn.ToString()); 
        
        // If that is null or empty, this must be running on the staging or production VM
        if (string.IsNullOrEmpty(connectionString))
        {
            string keyType = "";
            try
            {
                keyType = GetKeyType("isProduction").Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("No access to Keys in Azure KeyVault.");
            }
            
            // Only production VM has access to Keys, so get connection string Secret for production db
            if (keyType.Equals("RSA")) 
            {
                connectionString = await GetSecret(EnvVarKeys.dbconnprod.ToString());
            }
            else if (keyType.Equals(""))
            // No access, so get connection string Secret for staging db
            {
                connectionString = await GetSecret(EnvVarKeys.dbconn.ToString());
            }
        }

        return connectionString;
    }
    
    public static async Task<string> GetMailPassword()
    {
        var mailPassword = Environment.GetEnvironmentVariable(EnvVarKeys.MailPassword.ToString());
        if (string.IsNullOrEmpty(mailPassword))
        {
            mailPassword = await GetSecret(EnvVarKeys.MailPassword.ToString());
        }

        return mailPassword;
    }
    
    public static async Task<string> GetMqttToken()
    {
        var mqttToken = Environment.GetEnvironmentVariable(EnvVarKeys.MqttToken.ToString());
        if (string.IsNullOrEmpty(mqttToken))
        {
            mqttToken = await GetSecret(EnvVarKeys.MqttToken.ToString());
        }

        return mqttToken;
    }
    
    public static async Task<string> GetKeyType(string keyName)
    {
        var kvUri = "https://climatekv.vault.azure.net/";
        var client = new KeyClient(new Uri(kvUri), new DefaultAzureCredential());
        try
        {
            KeyVaultKey key = await client.GetKeyAsync(keyName);
            return key.KeyType.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred while retrieving key from Azure Key Vault: {e.Message}");
            return null;
        }
    }
}
