using System.Security.Authentication;
using infrastructure.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;

namespace api.security;

public class TokenService
{
    public string IssueJwt(EndUser user)
    {
        try
        {
            IJwtAlgorithm algorithm = new HMACSHA512Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(user, Environment.GetEnvironmentVariable("FULLSTACK_JWT_PRIVATE_KEY") ?? "alskjdhffffflisdjfnidssssjf948jfoiejfs9jv9s84roisjdmvlkxdjvcs4jfdslvjdsfls4jdlivjdslifs");
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("User authentication succeeded, but could not create token");
        }
    }

    public Dictionary<string, string> ValidateJwtAndReturnClaims(string jwt)
    {
        try
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, new HMACSHA512Algorithm());
            
            //todo JWT private key should be saved and loaded in drone and azure
            var json = decoder.Decode(jwt, Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY"));
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!;
        }
        catch (Exception e)
        {
            //todo should be Logged and caught in a global exception handler. 
            throw new AuthenticationException("Authentication failed.");
        }
    }
}