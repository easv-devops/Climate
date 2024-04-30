using System.Security.Authentication;
using api.helpers;
using infrastructure.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using service.services;

namespace api.security;

public class TokenService
{
    public string IssueJwt(int userId)
    {
        
        try
        {
            string JwtKey = Environment.GetEnvironmentVariable(EnvVarKeys.JwtKey.ToString());
            if (ReferenceEquals(JwtKey, ""))
            {
                JwtKey = new KeyVaultService().GetSecret("JwtKey");
            }
            
            IJwtAlgorithm algorithm = new HMACSHA512Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(userId, JwtKey);
        }
        catch (Exception e)
        {
            //todo should be Logged and caught in a global exception handler. 
            throw new InvalidOperationException("User authentication succeeded, but could not create token");
        }
    }

    public Dictionary<string, string> ValidateJwtAndReturnClaims(string jwt)
    {
        try
        {
            string JwtKey = Environment.GetEnvironmentVariable(EnvVarKeys.JwtKey.ToString());
            if (ReferenceEquals(JwtKey, ""))
            {
                JwtKey = new KeyVaultService().GetSecret("JwtKey");
            }
            
            IJsonSerializer serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, new HMACSHA512Algorithm());
            var json = decoder.Decode(jwt, JwtKey);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!;
        }
        catch (Exception e)
        {
            //todo should be Logged and caught in a global exception handler. 
            throw new AuthenticationException("Authentication failed.");
        }
    }
}