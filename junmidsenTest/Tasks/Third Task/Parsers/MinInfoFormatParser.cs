using System.Globalization;
using System.Text.RegularExpressions;


using junmidsenTest.Tasks.Third_Task.Interfaces;
using junmidsenTest.Tasks.Third_Task.Models;


namespace junmidsenTest.Tasks.Third_Task.Parsers;


public class MinInfoFormatParser : BaseFormatParser, ILogParser
{

    private static readonly Regex Pattern = new Regex(
           @"^(\d{2}\.\d{2}\.\d{4})\s+(\d{2}:\d{2}:\d{2}\.\d+)\s+(\w+)\s+(.+)$",
           RegexOptions.Compiled);

    public bool CanParse(string logLine)
    {
        return Pattern.IsMatch(logLine);
    }

    public LogEntry Parse(string logLine)
    {
        var match = Pattern.Match(logLine);
        if (!match.Success)
            throw new FormatException($"Invalid format : {logLine}");

        string dateStr = match.Groups[1].Value;
        string time = match.Groups[2].Value;
        string level = match.Groups[3].Value;
        string message = match.Groups[4].Value;

        DateTime date = DateTime.ParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        return new LogEntry
        {
            Date = date,
            Time = time,
            LogLevel = NormalizeLogLevel(level),
            CallingMethod = "DEFAULT",
            Message = message
        };
    }
}
