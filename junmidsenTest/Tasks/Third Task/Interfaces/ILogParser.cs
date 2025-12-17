using junmidsenTest.Tasks.Third_Task.Models;

namespace junmidsenTest.Tasks.Third_Task.Interfaces;

public interface ILogParser
{
    bool CanParse(string logLine);
    LogEntry Parse(string logLine);
}
