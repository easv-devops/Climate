using lib;
using infrastructure.Models;

namespace api.serverEventModels;

public class ServerSendsCountryCodes : BaseDto
{
    public IEnumerable<CountryCodeDto> CountryCode { get; set; }
}

