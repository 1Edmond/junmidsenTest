
namespace junmidsenTest.Tasks.Third_Task.Models;

public class LogEntry
{
    public DateTime Date { get; set; }
    public string Time { get; set; }
    public string LogLevel { get; set; }
    public string CallingMethod { get; set; }
    public string Message { get; set; }

    public string ToStandardFormat()
    {
        string formattedDate = Date.ToString("dd-MM-yyyy");
        return $"{formattedDate}\t{Time}\t{LogLevel}\t{CallingMethod}\t{Message}";
    }
}
