using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace Gear.Models
{
    public class WebOperations
    {
        public static string? GetData(string url)
        {
            string? result = null;
            try
            {
                HttpClient client = new();
                result = client.GetStringAsync(url).Result;
            }
            catch { }
            return result;
        }
    }

    public class WeatherApis
    {
        private static readonly string key = "ad768f25db9eb67e3883c2a16f59295b";
        private static readonly string ipSrc = "https://ip.useragentinfo.com/myip";
        private static readonly string amapBase = "https://restapi.amap.com/v3/";

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class IpInformation
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string Status { get; set; }
            public string Info { get; set; }
            public string Infocode { get; set; }
            public string Province { get; set; }
            public string City { get; set; }
            public string Adcode { get; set; }
            public string Rectangle { get; set; }
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Life
        {
            public string Province { get; set; }
            public string City { get; set; }
            public string Adcode { get; set; }
            public string Weather { get; set; }
            public string Temperature { get; set; }
            public string WindDirection { get; set; }
            public string WindPower { get; set; }
            public string Humidity { get; set; }
            public string ReportTime { get; set; }
        }

        public class WeatherInformation
        {
            public string Status { get; set; }
            public string Count { get; set; }
            public string Info { get; set; }
            public string Infocode { get; set; }
            public List<Life> Lives { get; set; }
        }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        public static string? GetHostIp()
        {
            string? ip = WebOperations.GetData(ipSrc);
            if (ip is not null)
                return ip[..];
            return null;
        }

        public static IpInformation? GetIpInformation(string ip)
        {
            string? json = WebOperations.GetData(amapBase + "ip?key=" + key + "&ip=" + ip);
            if (json is null) return null;
            IpInformation? i2 = new();
            try
            {
                i2 = JsonConvert.DeserializeObject<IpInformation>(json);
            }
            catch { }

            return i2 is null ? null : i2;
        }

        public static WeatherInformation? GetWeatherInformation(string adcode)
        {
            string? json = WebOperations.GetData($"{amapBase}weather/weatherInfo?key={key}&city={adcode}");
            if (json is null) return null;

            WeatherInformation? weather = JsonConvert.DeserializeObject<WeatherInformation>(json);

            return weather is null ? null : weather;
        }
    }

    public class ActionApis
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Actor
        {
            [JsonProperty("login")]
            public string Login { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("node_id")]
            public string NodeId { get; set; }

            [JsonProperty("avatar_url")]
            public string AvatarUrl { get; set; }

            [JsonProperty("gravatar_id")]
            public string GravatarId { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("html_url")]
            public string HtmlUrl { get; set; }

            [JsonProperty("followers_url")]
            public string FollowersUrl { get; set; }

            [JsonProperty("following_url")]
            public string FollowingUrl { get; set; }

            [JsonProperty("gists_url")]
            public string GistsUrl { get; set; }

            [JsonProperty("starred_url")]
            public string StarredUrl { get; set; }

            [JsonProperty("subscriptions_url")]
            public string SubscriptionsUrl { get; set; }

            [JsonProperty("organizations_url")]
            public string OrganizationsUrl { get; set; }

            [JsonProperty("repos_url")]
            public string ReposUrl { get; set; }

            [JsonProperty("events_url")]
            public string EventsUrl { get; set; }

            [JsonProperty("received_events_url")]
            public string ReceivedEventsUrl { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("site_admin")]
            public bool SiteAdmin { get; set; }
        }

        public class Author
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }
        }

        public class Committer
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }
        }

        public class HeadCommit
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("tree_id")]
            public string TreeId { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("timestamp")]
            public DateTime Timestamp { get; set; }

            [JsonProperty("author")]
            public Author Author { get; set; }

            [JsonProperty("committer")]
            public Committer Committer { get; set; }
        }

        public class HeadRepository
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("node_id")]
            public string NodeId { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("full_name")]
            public string FullName { get; set; }

            [JsonProperty("private")]
            public bool Private { get; set; }

            [JsonProperty("owner")]
            public Owner Owner { get; set; }

            [JsonProperty("html_url")]
            public string HtmlUrl { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("fork")]
            public bool Fork { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("forks_url")]
            public string ForksUrl { get; set; }

            [JsonProperty("keys_url")]
            public string KeysUrl { get; set; }

            [JsonProperty("collaborators_url")]
            public string CollaboratorsUrl { get; set; }

            [JsonProperty("teams_url")]
            public string TeamsUrl { get; set; }

            [JsonProperty("hooks_url")]
            public string HooksUrl { get; set; }

            [JsonProperty("issue_events_url")]
            public string IssueEventsUrl { get; set; }

            [JsonProperty("events_url")]
            public string EventsUrl { get; set; }

            [JsonProperty("assignees_url")]
            public string AssigneesUrl { get; set; }

            [JsonProperty("branches_url")]
            public string BranchesUrl { get; set; }

            [JsonProperty("tags_url")]
            public string TagsUrl { get; set; }

            [JsonProperty("blobs_url")]
            public string BlobsUrl { get; set; }

            [JsonProperty("git_tags_url")]
            public string GitTagsUrl { get; set; }

            [JsonProperty("git_refs_url")]
            public string GitRefsUrl { get; set; }

            [JsonProperty("trees_url")]
            public string TreesUrl { get; set; }

            [JsonProperty("statuses_url")]
            public string StatusesUrl { get; set; }

            [JsonProperty("languages_url")]
            public string LanguagesUrl { get; set; }

            [JsonProperty("stargazers_url")]
            public string StargazersUrl { get; set; }

            [JsonProperty("contributors_url")]
            public string ContributorsUrl { get; set; }

            [JsonProperty("subscribers_url")]
            public string SubscribersUrl { get; set; }

            [JsonProperty("subscription_url")]
            public string SubscriptionUrl { get; set; }

            [JsonProperty("commits_url")]
            public string CommitsUrl { get; set; }

            [JsonProperty("git_commits_url")]
            public string GitCommitsUrl { get; set; }

            [JsonProperty("comments_url")]
            public string CommentsUrl { get; set; }

            [JsonProperty("issue_comment_url")]
            public string IssueCommentUrl { get; set; }

            [JsonProperty("contents_url")]
            public string ContentsUrl { get; set; }

            [JsonProperty("compare_url")]
            public string CompareUrl { get; set; }

            [JsonProperty("merges_url")]
            public string MergesUrl { get; set; }

            [JsonProperty("archive_url")]
            public string ArchiveUrl { get; set; }

            [JsonProperty("downloads_url")]
            public string DownloadsUrl { get; set; }

            [JsonProperty("issues_url")]
            public string IssuesUrl { get; set; }

            [JsonProperty("pulls_url")]
            public string PullsUrl { get; set; }

            [JsonProperty("milestones_url")]
            public string MilestonesUrl { get; set; }

            [JsonProperty("notifications_url")]
            public string NotificationsUrl { get; set; }

            [JsonProperty("labels_url")]
            public string LabelsUrl { get; set; }

            [JsonProperty("releases_url")]
            public string ReleasesUrl { get; set; }

            [JsonProperty("deployments_url")]
            public string DeploymentsUrl { get; set; }
        }

        public class Owner
        {
            [JsonProperty("login")]
            public string Login { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("node_id")]
            public string NodeId { get; set; }

            [JsonProperty("avatar_url")]
            public string AvatarUrl { get; set; }

            [JsonProperty("gravatar_id")]
            public string GravatarId { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("html_url")]
            public string HtmlUrl { get; set; }

            [JsonProperty("followers_url")]
            public string FollowersUrl { get; set; }

            [JsonProperty("following_url")]
            public string FollowingUrl { get; set; }

            [JsonProperty("gists_url")]
            public string GistsUrl { get; set; }

            [JsonProperty("starred_url")]
            public string StarredUrl { get; set; }

            [JsonProperty("subscriptions_url")]
            public string SubscriptionsUrl { get; set; }

            [JsonProperty("organizations_url")]
            public string OrganizationsUrl { get; set; }

            [JsonProperty("repos_url")]
            public string ReposUrl { get; set; }

            [JsonProperty("events_url")]
            public string EventsUrl { get; set; }

            [JsonProperty("received_events_url")]
            public string ReceivedEventsUrl { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("site_admin")]
            public bool SiteAdmin { get; set; }
        }

        public class Repository
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("node_id")]
            public string NodeId { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("full_name")]
            public string FullName { get; set; }

            [JsonProperty("private")]
            public bool Private { get; set; }

            [JsonProperty("owner")]
            public Owner Owner { get; set; }

            [JsonProperty("html_url")]
            public string HtmlUrl { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("fork")]
            public bool Fork { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("forks_url")]
            public string ForksUrl { get; set; }

            [JsonProperty("keys_url")]
            public string KeysUrl { get; set; }

            [JsonProperty("collaborators_url")]
            public string CollaboratorsUrl { get; set; }

            [JsonProperty("teams_url")]
            public string TeamsUrl { get; set; }

            [JsonProperty("hooks_url")]
            public string HooksUrl { get; set; }

            [JsonProperty("issue_events_url")]
            public string IssueEventsUrl { get; set; }

            [JsonProperty("events_url")]
            public string EventsUrl { get; set; }

            [JsonProperty("assignees_url")]
            public string AssigneesUrl { get; set; }

            [JsonProperty("branches_url")]
            public string BranchesUrl { get; set; }

            [JsonProperty("tags_url")]
            public string TagsUrl { get; set; }

            [JsonProperty("blobs_url")]
            public string BlobsUrl { get; set; }

            [JsonProperty("git_tags_url")]
            public string GitTagsUrl { get; set; }

            [JsonProperty("git_refs_url")]
            public string GitRefsUrl { get; set; }

            [JsonProperty("trees_url")]
            public string TreesUrl { get; set; }

            [JsonProperty("statuses_url")]
            public string StatusesUrl { get; set; }

            [JsonProperty("languages_url")]
            public string LanguagesUrl { get; set; }

            [JsonProperty("stargazers_url")]
            public string StargazersUrl { get; set; }

            [JsonProperty("contributors_url")]
            public string ContributorsUrl { get; set; }

            [JsonProperty("subscribers_url")]
            public string SubscribersUrl { get; set; }

            [JsonProperty("subscription_url")]
            public string SubscriptionUrl { get; set; }

            [JsonProperty("commits_url")]
            public string CommitsUrl { get; set; }

            [JsonProperty("git_commits_url")]
            public string GitCommitsUrl { get; set; }

            [JsonProperty("comments_url")]
            public string CommentsUrl { get; set; }

            [JsonProperty("issue_comment_url")]
            public string IssueCommentUrl { get; set; }

            [JsonProperty("contents_url")]
            public string ContentsUrl { get; set; }

            [JsonProperty("compare_url")]
            public string CompareUrl { get; set; }

            [JsonProperty("merges_url")]
            public string MergesUrl { get; set; }

            [JsonProperty("archive_url")]
            public string ArchiveUrl { get; set; }

            [JsonProperty("downloads_url")]
            public string DownloadsUrl { get; set; }

            [JsonProperty("issues_url")]
            public string IssuesUrl { get; set; }

            [JsonProperty("pulls_url")]
            public string PullsUrl { get; set; }

            [JsonProperty("milestones_url")]
            public string MilestonesUrl { get; set; }

            [JsonProperty("notifications_url")]
            public string NotificationsUrl { get; set; }

            [JsonProperty("labels_url")]
            public string LabelsUrl { get; set; }

            [JsonProperty("releases_url")]
            public string ReleasesUrl { get; set; }

            [JsonProperty("deployments_url")]
            public string DeploymentsUrl { get; set; }
        }

        public class Root
        {
            [JsonProperty("total_count")]
            public int TotalCount { get; set; }

            [JsonProperty("workflow_runs")]
            public List<WorkflowRun> WorkflowRuns { get; set; }
        }

        public class TriggeringActor
        {
            [JsonProperty("login")]
            public string Login { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("node_id")]
            public string NodeId { get; set; }

            [JsonProperty("avatar_url")]
            public string AvatarUrl { get; set; }

            [JsonProperty("gravatar_id")]
            public string GravatarId { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("html_url")]
            public string HtmlUrl { get; set; }

            [JsonProperty("followers_url")]
            public string FollowersUrl { get; set; }

            [JsonProperty("following_url")]
            public string FollowingUrl { get; set; }

            [JsonProperty("gists_url")]
            public string GistsUrl { get; set; }

            [JsonProperty("starred_url")]
            public string StarredUrl { get; set; }

            [JsonProperty("subscriptions_url")]
            public string SubscriptionsUrl { get; set; }

            [JsonProperty("organizations_url")]
            public string OrganizationsUrl { get; set; }

            [JsonProperty("repos_url")]
            public string ReposUrl { get; set; }

            [JsonProperty("events_url")]
            public string EventsUrl { get; set; }

            [JsonProperty("received_events_url")]
            public string ReceivedEventsUrl { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("site_admin")]
            public bool SiteAdmin { get; set; }
        }

        public class WorkflowRun
        {
            [JsonProperty("id")]
            public object Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("node_id")]
            public string NodeId { get; set; }

            [JsonProperty("head_branch")]
            public string HeadBranch { get; set; }

            [JsonProperty("head_sha")]
            public string HeadSha { get; set; }

            [JsonProperty("path")]
            public string Path { get; set; }

            [JsonProperty("display_title")]
            public string DisplayTitle { get; set; }

            [JsonProperty("run_number")]
            public int RunNumber { get; set; }

            [JsonProperty("event")]
            public string Event { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("conclusion")]
            public string Conclusion { get; set; }

            [JsonProperty("workflow_id")]
            public int WorkflowId { get; set; }

            [JsonProperty("check_suite_id")]
            public object CheckSuiteId { get; set; }

            [JsonProperty("check_suite_node_id")]
            public string CheckSuiteNodeId { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("html_url")]
            public string HtmlUrl { get; set; }

            [JsonProperty("pull_requests")]
            public List<object> PullRequests { get; set; }

            [JsonProperty("created_at")]
            public DateTime CreatedAt { get; set; }

            [JsonProperty("updated_at")]
            public DateTime UpdatedAt { get; set; }

            [JsonProperty("actor")]
            public Actor Actor { get; set; }

            [JsonProperty("run_attempt")]
            public int RunAttempt { get; set; }

            [JsonProperty("referenced_workflows")]
            public List<object> ReferencedWorkflows { get; set; }

            [JsonProperty("run_started_at")]
            public DateTime RunStartedAt { get; set; }

            [JsonProperty("triggering_actor")]
            public TriggeringActor TriggeringActor { get; set; }

            [JsonProperty("jobs_url")]
            public string JobsUrl { get; set; }

            [JsonProperty("logs_url")]
            public string LogsUrl { get; set; }

            [JsonProperty("check_suite_url")]
            public string CheckSuiteUrl { get; set; }

            [JsonProperty("artifacts_url")]
            public string ArtifactsUrl { get; set; }

            [JsonProperty("cancel_url")]
            public string CancelUrl { get; set; }

            [JsonProperty("rerun_url")]
            public string RerunUrl { get; set; }

            [JsonProperty("previous_attempt_url")]
            public object PreviousAttemptUrl { get; set; }

            [JsonProperty("workflow_url")]
            public string WorkflowUrl { get; set; }

            [JsonProperty("head_commit")]
            public HeadCommit HeadCommit { get; set; }

            [JsonProperty("repository")]
            public Repository Repository { get; set; }

            [JsonProperty("head_repository")]
            public HeadRepository HeadRepository { get; set; }
        }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

    }

    public partial class WindowEnumerator
    {
        /// <summary>
        /// 查找当前用户空间下所有符合条件的窗口。如果不指定条件，将仅查找可见窗口。
        /// </summary>
        /// <param name="match">过滤窗口的条件。如果设置为 null，将仅查找可见窗口。</param>
        /// <returns>找到的所有窗口信息。</returns>
        //public static IReadOnlyList<WindowInfo> FindAll(Predicate<WindowInfo>? match = null)
        //{
        //    var windowList = new List<WindowInfo>();
        //    EnumWindows(OnWindowEnum, 0);
        //    return windowList.FindAll(match ?? DefaultPredicate);

        //    bool OnWindowEnum(IntPtr hWnd, int lparam)
        //    {
        //        // 仅查找顶层窗口。
        //        if (GetParent(hWnd) == IntPtr.Zero)
        //        {
        //            // 获取窗口类名。
        //            var lpString = new StringBuilder(512);
        //            GetClassName(hWnd, lpString, lpString.Capacity);
        //            var className = lpString.ToString();

        //            // 获取窗口标题。
        //            var lptrString = new StringBuilder(512);
        //            GetWindowText(hWnd, lptrString, lptrString.Capacity);
        //            var title = lptrString.ToString().Trim();

        //            // 获取窗口可见性。
        //            var isVisible = IsWindowVisible(hWnd);

        //            // 获取窗口位置和尺寸。
        //            LPRECT rect = default;
        //            GetWindowRect(hWnd, ref rect);
        //            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

        //            // 添加到已找到的窗口列表。
        //            windowList.Add(new WindowInfo(hWnd, className, title, isVisible, bounds));
        //        }

        //        return true;
        //    }
        //}

        /// <summary>
        /// 默认的查找窗口的过滤条件。可见 + 非最小化 + 包含窗口标题。
        /// </summary>
        //private static readonly Predicate<WindowInfo> DefaultPredicate = x => x.IsVisible && !x.IsMinimized && x.Title.Length > 0;

        private delegate bool WndEnumProc(IntPtr hWnd, int lParam);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(WndEnumProc lpEnumFunc, int lParam);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lptrString, int nMaxCount);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref LPRECT rect);

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct LPRECT
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }
    }

    /// <summary>
    /// 获取 Win32 窗口的一些基本信息。
    /// </summary>
    public readonly struct WindowInfo
    {
        public WindowInfo(IntPtr hWnd, string className, string title, bool isVisible, Rectangle bounds) : this()
        {
            Hwnd = hWnd;
            ClassName = className;
            Title = title;
            IsVisible = isVisible;
            Bounds = bounds;
        }

        /// <summary>
        /// 获取窗口句柄。
        /// </summary>
        public IntPtr Hwnd { get; }

        /// <summary>
        /// 获取窗口类名。
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// 获取窗口标题。
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 获取当前窗口是否可见。
        /// </summary>
        public bool IsVisible { get; }

        /// <summary>
        /// 获取窗口当前的位置和尺寸。
        /// </summary>
        public Rectangle Bounds { get; }

        /// <summary>
        /// 获取窗口当前是否是最小化的。
        /// </summary>
        public bool IsMinimized => Bounds.Left == -32000 && Bounds.Top == -32000;
    }
}
