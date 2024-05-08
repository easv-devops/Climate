using System.Security.Authentication;
using infrastructure;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;

namespace service.services;

public class TokenService
{
    public async Task<string> IssueJwt(int userId)
    {
        try
        {
            IJwtAlgorithm algorithm = new HMACSHA512Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            // Calculate the expiration time (e.g., 1 day from now)
            DateTime expirationTime = DateTime.UtcNow.AddDays(30);

            // Add the expiration time to the token payload
            var payload = new Dictionary<string, object>
            {
                { "sub", userId },
                { "exp", new DateTimeOffset(expirationTime).ToUnixTimeSeconds() } // Convert expiration time to UNIX timestamp
            };

            // Encode the token with the updated payload
            return encoder.Encode(payload, await KeyVaultService.GetToken());
        }
        catch (Exception e)
        {
            // Handle exceptions
            throw new InvalidOperationException("User authentication succeeded, but could not create token", e);
        }
    }

    public async Task<Dictionary<string, string>> ValidateJwtAndReturnClaims(string jwt)
    {
        try
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, new HMACSHA512Algorithm());
            var json = decoder.Decode(jwt, await KeyVaultService.GetToken());
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!;
        }
        catch (Exception e)
        {
            //todo should be Logged and caught in a global exception handler. 
            throw new AuthenticationException("Authentication failed.");
        }
    }
}