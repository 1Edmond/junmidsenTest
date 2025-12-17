using junmidsenTest.Tasks.Third_Task.Models;
using junmidsenTest.Tasks.Third_Task.Parsers;

namespace junmidsenTest.Test.Tests;

public class MaxInfoFormatParserTests
{
    private readonly MaxInfoFormatParser _parser;

    public MaxInfoFormatParserTests()
    {
        _parser = new MaxInfoFormatParser();
    }

    [Fact]
    public void CanParse_ValidFormat2_ReturnsTrue()
    {
        string logLine = "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства";

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
    public void Parse_ValidFormat2_ReturnsCorrectLogEntry()
    {
        string logLine = "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";

        LogEntry entry = _parser.Parse(logLine);

        Assert.Equal(new DateTime(2025, 3, 10), entry.Date);
        Assert.Equal("15:14:51.5882", entry.Time);
        Assert.Equal("INFO", entry.LogLevel);
        Assert.Equal("MobileComputer.GetDeviceId", entry.CallingMethod);
        Assert.Equal("Код устройства: '@MINDEO-M40-D-410244015546'", entry.Message);
    }

    [Theory]
    [InlineData("2025-03-10 15:14:51.5882| INFO|11|Method| Message", "INFO")]
    [InlineData("2025-03-10 15:14:51.5882| WARN|11|Method| Message", "WARN")]
    [InlineData("2025-03-10 15:14:51.5882| ERROR|11|Method| Message", "ERROR")]
    [InlineData("2025-03-10 15:14:51.5882| DEBUG|11|Method| Message", "DEBUG")]
    public void Parse_DifferentLogLevels_NormalizesCorrectly(string logLine, string expectedLevel)
    {
        LogEntry entry = _parser.Parse(logLine);

        Assert.Equal(expectedLevel, entry.LogLevel);
    }
}
