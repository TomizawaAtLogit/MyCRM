using System.Globalization;

namespace AspireApp1.Web.Services;

public class LocalizationService
{
    private readonly ILogger<LocalizationService> _logger;
    private string _currentLanguage = "en";
    
    public event Action? OnLanguageChanged;
    
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
            ["Japanese"] = "Japanese",
            ["Audit Logs"] = "Audit Logs",
            ["Filter Audit Logs"] = "Filter Audit Logs",
            ["Entity Type"] = "Entity Type",
            ["Action"] = "Action",
            ["From Date"] = "From Date",
            ["To Date"] = "To Date",
            ["Search"] = "Search",
            ["Clear"] = "Clear",
            ["All Types"] = "All Types",
            ["All Actions"] = "All Actions",
            ["Loading audit logs..."] = "Loading audit logs...",
            ["No audit logs found matching the filters."] = "No audit logs found matching the filters.",
            ["Username, entity name, etc."] = "Username, entity name, etc.",
            // Projects page
            ["Filters"] = "Filters",
            ["Customer"] = "Customer",
            ["All Customers"] = "All Customers",
            ["Status"] = "Status",
            ["All Statuses"] = "All Statuses",
            ["Existing Projects"] = "Existing Projects",
            ["Loading..."] = "Loading...",
            ["No projects."] = "No projects.",
            ["Name"] = "Name",
            ["Created"] = "Created",
            ["Actions"] = "Actions",
            ["Create Project"] = "Create Project",
            // Orders page
            ["Orders / Contracts"] = "Orders / Contracts",
            ["Filter Orders"] = "Filter Orders",
            ["Order number, customer..."] = "Order number, customer...",
            ["Contract Type"] = "Contract Type",
            ["All Status"] = "All Status",
            ["Start Date From"] = "Start Date From",
            ["Start Date To"] = "Start Date To",
            ["Loading orders..."] = "Loading orders...",
            ["No orders found matching the filters."] = "No orders found matching the filters.",
            ["Orders List"] = "Orders List",
            ["total"] = "total",
            ["Items per page:"] = "Items per page:",
            // Customers page
            ["Loading customers..."] = "Loading customers...",
            ["No customers found."] = "No customers found.",
            ["Contact:"] = "Contact:",
            ["Add New Customer"] = "Add New Customer",
            ["Back to List"] = "Back to List",
            ["Contact Person"] = "Contact Person",
            // Common
            ["Activity Calendar"] = "Activity Calendar",
            ["Activities for"] = "Activities for",
            ["Calendar"] = "Calendar",
            ["List"] = "List",
            ["View Project"] = "View Project"
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
            ["Japanese"] = "日本語",
            ["Audit Logs"] = "監査ログ",
            ["Filter Audit Logs"] = "監査ログをフィルタ",
            ["Entity Type"] = "エンティティタイプ",
            ["Action"] = "アクション",
            ["From Date"] = "開始日",
            ["To Date"] = "終了日",
            ["Search"] = "検索",
            ["Clear"] = "クリア",
            ["All Types"] = "すべてのタイプ",
            ["All Actions"] = "すべてのアクション",
            ["Loading audit logs..."] = "監査ログを読み込み中...",
            ["No audit logs found matching the filters."] = "フィルタに一致する監査ログが見つかりません。",
            ["Username, entity name, etc."] = "ユーザー名、エンティティ名など",
            // Projects page
            ["Filters"] = "フィルタ",
            ["Customer"] = "顧客",
            ["All Customers"] = "すべての顧客",
            ["Status"] = "ステータス",
            ["All Statuses"] = "すべてのステータス",
            ["Existing Projects"] = "既存のプロジェクト",
            ["Loading..."] = "読み込み中...",
            ["No projects."] = "プロジェクトがありません。",
            ["Name"] = "名前",
            ["Created"] = "作成日",
            ["Actions"] = "アクション",
            ["Create Project"] = "プロジェクトを作成",
            // Orders page
            ["Orders / Contracts"] = "注文 / 契約",
            ["Filter Orders"] = "注文をフィルタ",
            ["Order number, customer..."] = "注文番号、顧客名など...",
            ["Contract Type"] = "契約タイプ",
            ["All Status"] = "すべてのステータス",
            ["Start Date From"] = "開始日（開始）",
            ["Start Date To"] = "開始日（終了）",
            ["Loading orders..."] = "注文を読み込み中...",
            ["No orders found matching the filters."] = "フィルタに一致する注文が見つかりません。",
            ["Orders List"] = "注文リスト",
            ["total"] = "件",
            ["Items per page:"] = "ページあたりの件数:",
            // Customers page
            ["Loading customers..."] = "顧客を読み込み中...",
            ["No customers found."] = "顧客が見つかりません。",
            ["Contact:"] = "連絡先:",
            ["Add New Customer"] = "新規顧客を追加",
            ["Back to List"] = "リストに戻る",
            ["Contact Person"] = "担当者",
            // Common
            ["Activity Calendar"] = "アクティビティカレンダー",
            ["Activities for"] = "アクティビティ：",
            ["Calendar"] = "カレンダー",
            ["List"] = "リスト",
            ["View Project"] = "プロジェクトを表示"
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
            
            // Notify all subscribers that language has changed
            OnLanguageChanged?.Invoke();
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
