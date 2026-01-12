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
            ["Cases"] = "Cases",
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
            // Cases page
            ["Existing Cases"] = "Existing Cases",
            ["No cases."] = "No cases.",
            ["Create Case"] = "Create Case",
            ["Title"] = "Title",
            ["Priority"] = "Priority",
            ["All Priorities"] = "All Priorities",
            ["Low"] = "Low",
            ["Medium"] = "Medium",
            ["High"] = "High",
            ["Critical"] = "Critical",
            ["Issue Type"] = "Issue Type",
            ["Bug"] = "Bug",
            ["Incident"] = "Incident",
            ["Service Request"] = "Service Request",
            ["Question"] = "Question",
            ["Maintenance"] = "Maintenance",
            ["Assigned To"] = "Assigned To",
            ["Due Date"] = "Due Date",
            ["Resolution Notes"] = "Resolution Notes",
            ["Open"] = "Open",
            ["In Progress"] = "In Progress",
            ["Pending"] = "Pending",
            ["Resolved"] = "Resolved",
            ["Closed"] = "Closed",
            ["Overdue"] = "Overdue",
            ["SLA Deadline"] = "SLA Deadline",
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
            ["Previous"] = "Previous",
            ["Next"] = "Next",
            ["List"] = "List",
            ["View Project"] = "View Project",
            // Activity
            ["Add New Activity"] = "Add New Activity",
            ["New Activity"] = "New Activity",
            ["Activity Date"] = "Activity Date",
            ["Task"] = "Task",
            ["Summary"] = "Summary",
            ["Description"] = "Description",
            ["Next Action"] = "Next Action",
            ["Note"] = "Note",
            ["Performed By"] = "Performed By",
            ["Create Activity"] = "Create Activity",
            ["Activities"] = "Activities",
            ["Files"] = "Files",
            ["Loading activities..."] = "Loading activities...",
            ["No activities yet."] = "No activities yet.",
            ["Apply"] = "Apply",
            ["Save Changes"] = "Save Changes",
            ["Date"] = "Date",
            ["All"] = "All",
            ["From"] = "From",
            ["To"] = "To",
            ["Edit Activity"] = "Edit Activity",
            ["Cancel"] = "Cancel"
            // Scheduled task labels
            , ["Issue"] = "Issue"
            , ["Scheduled Task"] = "Scheduled Task"
            , ["Add Task"] = "Add Task"
            , ["New Task"] = "New Task"
            , ["Create Task"] = "Create Task"
            , ["No tasks yet."] = "No tasks yet."
            , ["Loading tasks..."] = "Loading tasks..."
            , ["Timeline"] = "Timeline"
            , ["Start"] = "Start"
            , ["End"] = "End"
            , ["Not Started"] = "Not Started"
            , ["Completed"] = "Completed"
            , ["Blocked"] = "Blocked"
            , ["Edit Task"] = "Edit Task"
        },
        ["ja"] = new Dictionary<string, string>
        {
            ["Home"] = "ホーム",
            ["Projects"] = "プロジェクト",
            ["Cases"] = "ケース",
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
            // Cases page
            ["Existing Cases"] = "既存のケース",
            ["No cases."] = "ケースがありません。",
            ["Create Case"] = "ケースを作成",
            ["Title"] = "タイトル",
            ["Priority"] = "優先度",
            ["All Priorities"] = "すべての優先度",
            ["Low"] = "低",
            ["Medium"] = "中",
            ["High"] = "高",
            ["Critical"] = "緊急",
            ["Issue Type"] = "問題タイプ",
            ["Bug"] = "バグ",
            ["Incident"] = "インシデント",
            ["Service Request"] = "サービスリクエスト",
            ["Question"] = "質問",
            ["Maintenance"] = "メンテナンス",
            ["Assigned To"] = "担当者",
            ["Due Date"] = "期限",
            ["Resolution Notes"] = "解決メモ",
            ["Open"] = "オープン",
            ["In Progress"] = "進行中",
            ["Pending"] = "保留中",
            ["Resolved"] = "解決済み",
            ["Closed"] = "クローズ",
            ["Overdue"] = "期限切れ",
            ["SLA Deadline"] = "SLA期限",
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
            ["Previous"] = "前へ",
            ["Next"] = "次へ",
            ["List"] = "リスト",
            ["View Project"] = "プロジェクトを表示",
            // Activity
            ["Add New Activity"] = "アクティビティを追加",
            ["New Activity"] = "新しいアクティビティ",
            ["Activity Date"] = "活動日",
            ["Task"] = "タスク",
            ["Summary"] = "サマリー",
            ["Description"] = "説明",
            ["Next Action"] = "次のアクション",
            ["Note"] = "メモ",
            ["Performed By"] = "担当者",
            ["Create Activity"] = "アクティビティを作成",
            ["Activities"] = "アクティビティ",
            ["Files"] = "ファイル",
            ["Loading activities..."] = "アクティビティを読み込み中…",
            ["No activities yet."] = "まだアクティビティはありません。",
            ["Apply"] = "適用",
            ["Save Changes"] = "変更を保存",
            ["Date"] = "日付",
            ["All"] = "すべて",
            ["From"] = "開始",
            ["To"] = "終了",
            ["Edit Activity"] = "アクティビティを編集",
            ["Cancel"] = "キャンセル"
            // Scheduled task labels
            , ["Issue"] = "課題"
            , ["Scheduled Task"] = "スケジュールタスク"
            , ["Add Task"] = "タスクを追加"
            , ["New Task"] = "新しいタスク"
            , ["Create Task"] = "タスクを作成"
            , ["No tasks yet."] = "まだタスクはありません。"
            , ["Loading tasks..."] = "タスクを読み込み中..."
            , ["Timeline"] = "タイムライン"
            , ["Start"] = "開始"
            , ["End"] = "終了"
            , ["Not Started"] = "未着手"
            , ["Completed"] = "完了"
            , ["Blocked"] = "ブロック済み"
            , ["Edit Task"] = "タスクを編集"
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
