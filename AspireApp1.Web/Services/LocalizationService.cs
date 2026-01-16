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
            ["Pre-sales"] = "Pre-sales",
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
            ["Planned Work"] = "Planned Work",
            ["Power Outage"] = "Power Outage",
            ["Other"] = "Other",
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
            ["DeleteCustomerConfirmation"] = "Are you sure you want to delete this customer?",
            ["DeleteCustomerBlocked"] = "Customer has related cases, projects, requirement definitions, or pre-sales proposals and cannot be deleted.",
            ["DeleteCustomerFailed"] = "Failed to delete customer.",
            // Common
            ["Activity Calendar"] = "Activity Calendar",
            ["Activities for"] = "Activities for",
            ["Calendar"] = "Calendar",
            ["Gantt"] = "Gantt",
            ["Daily"] = "Daily",
            ["Weekly"] = "Weekly",
            ["Monthly"] = "Monthly",
            ["Yearly"] = "Yearly",
            ["Week of"] = "Week of",
            ["Year of"] = "Year of",
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
            // Pre-sales page
            , ["Pre-sales Proposals"] = "Pre-sales Proposals"
            , ["Create Proposal"] = "Create Proposal"
            , ["Edit Proposal"] = "Edit Proposal"
            , ["Save Proposal"] = "Save Proposal"
            , ["Proposal Details"] = "Proposal Details"
            , ["Proposal created successfully!"] = "Proposal created successfully!"
            , ["Proposal updated successfully!"] = "Proposal updated successfully!"
            , ["Proposal deleted successfully!"] = "Proposal deleted successfully!"
            , ["Error deleting proposal"] = "Error deleting proposal"
            , ["Error saving proposal"] = "Error saving proposal"
            , ["View Proposal Details"] = "View Proposal Details"
            , ["Delete Proposal"] = "Delete Proposal"
            , ["Read Activity Details"] = "Read Activity Details"
            , ["Requirement Definition"] = "Requirement Definition"
            , ["Expected Close Date"] = "Expected Close Date"
            , ["Expected Close"] = "Expected Close"
            , ["Estimated Value"] = "Estimated Value"
            , ["Probability"] = "Probability"
            , ["Probability (%)"] = "Probability (%)"
            , ["Stage"] = "Stage"
            , ["All Stages"] = "All Stages"
            , ["All Users"] = "All Users"
            , ["No proposals found. Create one to get started!"] = "No proposals found. Create one to get started!"
            , ["Notes"] = "Notes"
            , ["ID"] = "ID"
            , ["Unassigned"] = "Unassigned"
            , ["Confirm Delete Proposal"] = "Are you sure you want to delete this proposal?"
            , ["Close"] = "Close"
            , ["Updated"] = "Updated"
            , ["PreSalesStatus.Draft"] = "Draft"
            , ["PreSalesStatus.InReview"] = "In Review"
            , ["PreSalesStatus.Pending"] = "Pending"
            , ["PreSalesStatus.Approved"] = "Approved"
            , ["PreSalesStatus.Rejected"] = "Rejected"
            , ["PreSalesStatus.Closed"] = "Closed"
            , ["PreSalesStage.InitialContact"] = "Initial Contact"
            , ["PreSalesStage.RequirementGathering"] = "Requirement Gathering"
            , ["PreSalesStage.ProposalDevelopment"] = "Proposal Development"
            , ["PreSalesStage.PresentationScheduled"] = "Presentation Scheduled"
            , ["PreSalesStage.NegotiationInProgress"] = "Negotiation In Progress"
            , ["PreSalesStage.AwaitingDecision"] = "Awaiting Decision"
            , ["PreSalesStage.Won"] = "Won"
            , ["PreSalesStage.Lost"] = "Lost"
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
            ["Pre-sales"] = "プレセールス",
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
            ["Issue Type"] = "タイプ",
            ["Bug"] = "バグ",
            ["Incident"] = "インシデント",
            ["Service Request"] = "サービスリクエスト",
            ["Question"] = "質問",
            ["Maintenance"] = "メンテナンス",
            ["Planned Work"] = "計画作業",
            ["Power Outage"] = "停電",
            ["Other"] = "その他",
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
            ["DeleteCustomerConfirmation"] = "この顧客を削除してもよろしいですか？",
            ["DeleteCustomerBlocked"] = "関連するケース、プロジェクト、要件定義、またはプレセールス提案が存在するため、この顧客を削除できません。",
            ["DeleteCustomerFailed"] = "顧客の削除に失敗しました。",
            // Common
            ["Activity Calendar"] = "アクティビティカレンダー",
            ["Activities for"] = "アクティビティ：",
            ["Calendar"] = "カレンダー",
            ["Gantt"] = "ガントチャート",
            ["Daily"] = "日次",
            ["Weekly"] = "週次",
            ["Monthly"] = "月次",
            ["Yearly"] = "年次",
            ["Week of"] = "週",
            ["Year of"] = "年",
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
            // Pre-sales page
            , ["Pre-sales Proposals"] = "プレセールス提案"
            , ["Create Proposal"] = "提案を作成"
            , ["Edit Proposal"] = "提案を編集"
            , ["Save Proposal"] = "提案を保存"
            , ["Proposal Details"] = "提案の詳細"
            , ["Proposal created successfully!"] = "提案が正常に作成されました！"
            , ["Proposal updated successfully!"] = "提案が正常に更新されました！"
            , ["Proposal deleted successfully!"] = "提案が正常に削除されました！"
            , ["Error deleting proposal"] = "提案の削除中にエラーが発生しました"
            , ["Error saving proposal"] = "提案の保存中にエラーが発生しました"
            , ["View Proposal Details"] = "提案の詳細を見る"
            , ["Delete Proposal"] = "提案を削除"
            , ["Read Activity Details"] = "アクティビティの詳細を読む"
            , ["Requirement Definition"] = "要件定義"
            , ["Expected Close Date"] = "想定完了日"
            , ["Expected Close"] = "想定完了"
            , ["Estimated Value"] = "推定金額"
            , ["Probability"] = "確率"
            , ["Probability (%)"] = "確率（％）"
            , ["Stage"] = "ステージ"
            , ["All Stages"] = "すべてのステージ"
            , ["All Users"] = "すべてのユーザー"
            , ["No proposals found. Create one to get started!"] = "提案が見つかりません。最初の提案を作成してください！"
            , ["Notes"] = "メモ"
            , ["ID"] = "ID"
            , ["Unassigned"] = "未割り当て"
            , ["Confirm Delete Proposal"] = "この提案を削除してもよろしいですか？"
            , ["Close"] = "閉じる"
            , ["Updated"] = "更新日"
            , ["PreSalesStatus.Draft"] = "下書き"
            , ["PreSalesStatus.InReview"] = "レビュー中"
            , ["PreSalesStatus.Pending"] = "保留"
            , ["PreSalesStatus.Approved"] = "承認済み"
            , ["PreSalesStatus.Rejected"] = "却下"
            , ["PreSalesStatus.Closed"] = "クローズ"
            , ["PreSalesStage.InitialContact"] = "初回コンタクト"
            , ["PreSalesStage.RequirementGathering"] = "要件収集"
            , ["PreSalesStage.ProposalDevelopment"] = "提案作成"
            , ["PreSalesStage.PresentationScheduled"] = "プレゼン予定"
            , ["PreSalesStage.NegotiationInProgress"] = "交渉中"
            , ["PreSalesStage.AwaitingDecision"] = "決定待ち"
            , ["PreSalesStage.Won"] = "受注"
            , ["PreSalesStage.Lost"] = "失注"
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
