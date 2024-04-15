using System.Security.Authentication;
using infrastructure.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;

namespace api.security;

public class TokenService
{
    public string IssueJwt(ShortUserDto user)
    {
        
        try
        {
            
            IJwtAlgorithm algorithm = new HMACSHA512Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(user, "2aX4QvDzexGuENuvVTpEc3t7mRdKJd9H3iO0CtjtlVU="); //todo should be a pgconn
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
            IJsonSerializer serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, new HMACSHA512Algorithm());
            var json = decoder.Decode(jwt, "2aX4QvDzexGuENuvVTpEc3t7mRdKJd9H3iO0CtjtlVU="); //todo should be a pgconn
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!;
        }
        catch (Exception e)
        {
            //todo should be Logged and caught in a global exception handler. 
            throw new AuthenticationException("Authentication failed.");
        }
    }
}