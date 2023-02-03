namespace Billiards.Web.Shared
{
    public class LogItem
    {
        public LogItem()
        {

        }

        public LogItem(string name, double time) : this(name, time.ToString("F0"))
        {
        }

        public LogItem(string name, string message)
        {
            Name = name;
            Message = message;
        }

        public string Name { get; set; }
        public string Message { get; set; }

    }
}
