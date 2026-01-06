using System.Globalization;

namespace AspireApp1.Web.Services;

public class LocalizationService
{
    private readonly ILogger<LocalizationService> _logger;
    private string _currentLanguage = "en";
    
    private readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        ["en"] = new Dictionary<string, string>
        {
            ["Home"] = "Home",
            ["Projects"] = "Projects",
            ["Customers"] = "Customers",
            ["Orders"] = "Orders",
            ["Audit"] = "Audit",
            ["Admin"] = "Admin",
            ["Language"] = "Language",
            ["English"] = "English",
            ["Japanese"] = "Japanese"
        },
        ["ja"] = new Dictionary<string, string>
        {
            ["Home"] = "ホーム",
            ["Projects"] = "プロジェクト",
            ["Customers"] = "顧客",
            ["Orders"] = "注文",
            ["Audit"] = "監査",
            ["Admin"] = "管理者",
            ["Language"] = "言語",
            ["English"] = "英語",
            ["Japanese"] = "日本語"
        }
    };

    public LocalizationService(ILogger<LocalizationService> logger)
    {
        _logger = logger;
    }

    public string GetString(string key)
    {
        try
        {
            if (_translations.TryGetValue(_currentLanguage, out var languageDict) &&
                languageDict.TryGetValue(key, out var translation))
            {
                return translation;
            }
            
            // Fallback to English
            if (_translations.TryGetValue("en", out var englishDict) &&
                englishDict.TryGetValue(key, out var englishTranslation))
            {
                return englishTranslation;
            }
            
            return key;
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
            _currentLanguage = cultureName;
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            _logger.LogInformation("Culture set to {Culture}", cultureName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting culture to {Culture}", cultureName);
        }
    }

    public string GetCurrentCulture()
    {
        return _currentLanguage;
    }
}
