using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace service.services;

public class KeyVaultService
{
    public string GetSecret(string secretName)
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

        KeyVaultSecret secret = client.GetSecret(secretName);
        System.Threading.Thread.Sleep(5000);
        return secret.Value;
    }
}