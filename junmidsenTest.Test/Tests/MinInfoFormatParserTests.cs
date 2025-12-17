using junmidsenTest.Tasks.Third_Task.Models;
using junmidsenTest.Tasks.Third_Task.Parsers;

namespace junmidsenTest.Test.Tests;

public class MinInfoFormatParserTests
{
    private readonly MinInfoFormatParser _parser;

    public MinInfoFormatParserTests()
    {
        _parser = new ();
    }

    [Fact]
    public void CanParse_ValidFormat1_ReturnsTrue()
    {
        string logLine = "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'";

        bool result = _parser.CanParse(logLine);

        Assert.True(result);
    }

    [Fact]
    public void CanParse_InvalidFormat_ReturnsFalse()
    {
        string logLine = "Invalid log line";

        bool result = _parser.CanParse(logLine);

        Assert.False(result);
    }

    [Fact]
    public void Parse_ValidFormat1_ReturnsCorrectLogEntry()
    {
        string logLine = "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'";

        LogEntry entry = _parser.Parse(logLine);

        Assert.Equal(new DateTime(2025, 3, 10), entry.Date);
        Assert.Equal("15:14:49.523", entry.Time);
        Assert.Equal("INFO", entry.LogLevel);
        Assert.Equal("DEFAULT", entry.CallingMethod);
        Assert.Equal("Версия программы: '3.4.0.48729'", entry.Message);
    }

    [Theory]
    [InlineData("10.03.2025 15:14:49.523 INFORMATION Message", "INFO")]
    [InlineData("10.03.2025 15:14:49.523 WARNING Message", "WARN")]
    [InlineData("10.03.2025 15:14:49.523 ERROR Message", "ERROR")]
    [InlineData("10.03.2025 15:14:49.523 DEBUG Message", "DEBUG")]
    public void Parse_DifferentLogLevels_NormalizesCorrectly(string logLine, string expectedLevel)
    {
        LogEntry entry = _parser.Parse(logLine);

        Assert.Equal(expectedLevel, entry.LogLevel);
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        string logLine = "Invalid format";

        Assert.Throws<FormatException>(() => _parser.Parse(logLine));
    }
}
