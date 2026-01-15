namespace AspireApp1.Web.Services;

public class GoogleMapsService
{
    private readonly IConfiguration _configuration;

    public GoogleMapsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetApiKey()
    {
        return _configuration["GoogleMaps:ApiKey"] ?? string.Empty;
    }
}
