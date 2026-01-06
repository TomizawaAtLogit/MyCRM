using System.Globalization;
using Microsoft.Extensions.Localization;

namespace AspireApp1.Web.Services;

public class LocalizationService
{
    private readonly IStringLocalizerFactory _localizerFactory;
    private readonly ILogger<LocalizationService> _logger;
    private IStringLocalizer? _localizer;

    public LocalizationService(IStringLocalizerFactory localizerFactory, ILogger<LocalizationService> logger)
    {
        _localizerFactory = localizerFactory;
        _logger = logger;
        InitializeLocalizer();
    }

    private void InitializeLocalizer()
    {
        try
        {
            _localizer = _localizerFactory.Create("Localization", "AspireApp1.FrontEnd");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize localizer");
        }
    }

    public string GetString(string key)
    {
        try
        {
            return _localizer?[key] ?? key;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting localized string for key {Key}", key);
            return key;
        }
    }

    public void SetCulture(string cultureName)
    {
        try
        {
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            InitializeLocalizer();
            _logger.LogInformation("Culture set to {Culture}", cultureName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting culture to {Culture}", cultureName);
        }
    }

    public string GetCurrentCulture()
    {
        return CultureInfo.CurrentUICulture.Name;
    }
}
