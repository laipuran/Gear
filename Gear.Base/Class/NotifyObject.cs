namespace Gear.Base.Class
{
    public enum ContentForm
    {
        Text,
        Formula
    }

    public class NotifyObject
    {
        public NotifyObject(ContentForm mode, string content)
        {
            DisplayMode = mode;
            Content = content;
        }

        public ContentForm DisplayMode { get; set; }
        public string Content { get; set; }
    }
}
