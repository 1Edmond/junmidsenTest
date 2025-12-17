using System.Globalization;
using System.Text.RegularExpressions;


using junmidsenTest.Tasks.Third_Task.Interfaces;
using junmidsenTest.Tasks.Third_Task.Models;


namespace junmidsenTest.Tasks.Third_Task.Parsers;

public class MaxInfoFormatParser : BaseFormatParser, ILogParser
{
    private static readonly Regex Pattern = new Regex(
            @"^(\d{4}-\d{2}-\d{2})\s+(\d{2}:\d{2}:\d{2}\.\d+)\|\s*(\w+)\|\d+\|([^|]+)\|\s*(.+)$",
            RegexOptions.Compiled);

    public bool CanParse(string logLine)
    {
        return Pattern.IsMatch(logLine);
    }

    public LogEntry Parse(string logLine)
    {
        var match = Pattern.Match(logLine);
        if (!match.Success)
            throw new FormatException($"Invalid format: {logLine}");

        string dateStr = match.Groups[1].Value;
        string time = match.Groups[2].Value;
        string level = match.Groups[3].Value.Trim();
        string method = match.Groups[4].Value.Trim();
        string message = match.Groups[5].Value.Trim();

        DateTime date = DateTime.ParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        return new LogEntry
        {
            Date = date,
            Time = time,
            LogLevel = NormalizeLogLevel(level),
            CallingMethod = method,
            Message = message
        };
    }
}
