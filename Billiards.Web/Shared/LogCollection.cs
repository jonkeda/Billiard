using System.Collections.ObjectModel;

namespace Billiards.Web.Shared;

public class LogCollection : Collection<LogItem>
{
    private DateTime begin;
    private DateTime now;

    public LogCollection()
    {
        LogName = "";
    }

    public LogCollection(string logName)
    {
        LogName = logName + " ";
    }

    private string LogName;

    public void Start()
    {
        Clear();
        now = DateTime.Now;
        begin = DateTime.Now;
    }

    public void Add(string name)
    {
        Add(new LogItem(LogName + name, (DateTime.Now - now).TotalMilliseconds));
        now = DateTime.Now;
    }

    public void Add(string name, string message)
    {
        Add(new LogItem(LogName + name, message));
    }

    public void Add(string name, long number)
    {
        Add(new LogItem(LogName + name, number.ToString()));
    }

    public void Add(LogCollection? log)
    {
        if (log == null)
        {
            return;
        }

        foreach (LogItem logItem in log)
        {
            Add(logItem);
        }
    }

    public void End()
    {
        Add(new LogItem("End", (DateTime.Now - begin).TotalMilliseconds));
    }
}