using System.Text;


using junmidsenTest.Tasks.Third_Task.Interfaces;
using junmidsenTest.Tasks.Third_Task.Models;
using junmidsenTest.Tasks.Third_Task.Parsers;


namespace junmidsenTest.Tasks.Third_Task;

public class LogStandardizer
{

    private readonly List<ILogParser> _parsers;
    private readonly string _outputPath;
    private readonly string _problemsPath;

    public LogStandardizer(string outputPath, string problemsPath = "problems.txt")
    {
        _parsers = new List<ILogParser>
            {
                new MinInfoFormatParser(),
                new MaxInfoFormatParser()
            };
        _outputPath = outputPath;
        _problemsPath = problemsPath;
    }

    public ProcessingStats ProcessLogFile(string inputPath)
    {
        if (!File.Exists(inputPath))
            throw new FileNotFoundException($"File not found : {inputPath}");

        var stats = new ProcessingStats();
        var validEntries = new List<string>();
        var problemEntries = new List<string>();

        string[] lines = File.ReadAllLines(inputPath, Encoding.UTF8);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                LogEntry entry = ParseLogLine(line);
                validEntries.Add(entry.ToStandardFormat());
                stats.SuccessCount++;
            }
            catch
            {
                problemEntries.Add(line);
                stats.ErrorCount++;
            }
        }

        if (validEntries.Any())
        {
            File.WriteAllLines(_outputPath, validEntries, Encoding.UTF8);
        }

        if (problemEntries.Any())
        {
            File.WriteAllLines(_problemsPath, problemEntries, Encoding.UTF8);
        }

        stats.TotalLines = lines.Length;
        return stats;
    }

    private LogEntry ParseLogLine(string line)
    {
        var parser = _parsers.FirstOrDefault(p => p.CanParse(line));
        
        if (parser != null)
        {
            return parser.Parse(line);
        }

        throw new FormatException($"Unrecognized format : {line}");
    }
}
