using junmidsenTest.Tasks.Third_Task.Models;


namespace junmidsenTest.Test;

public class LogEntryTests
{
    [Fact]
    public void ToStandardFormat_ValidEntry_ReturnsCorrectFormat()
    {
        var entry = new LogEntry
        {
            Date = new DateTime(2025, 3, 10),
            Time = "15:14:49.523",
            LogLevel = "INFO",
            CallingMethod = "DEFAULT",
            Message = "Test message"
        };

        string result = entry.ToStandardFormat();

        Assert.Equal("10-03-2025\t15:14:49.523\tINFO\tDEFAULT\tTest message", result);
    }

    [Fact]
    public void ToStandardFormat_WithCallingMethod_IncludesMethod()
    {
        var entry = new LogEntry
        {
            Date = new DateTime(2025, 3, 10),
            Time = "15:14:51.5882",
            LogLevel = "INFO",
            CallingMethod = "MobileComputer.GetDeviceId",
            Message = "Device code"
        };

        string result = entry.ToStandardFormat();

        Assert.Contains("MobileComputer.GetDeviceId", result);
    }

    [Fact]
    public void ToStandardFormat_DateFormat_IsCorrect()
    {
        var entry = new LogEntry
        {
            Date = new DateTime(2025, 12, 5),
            Time = "10:30:15.123",
            LogLevel = "DEBUG",
            CallingMethod = "DEFAULT",
            Message = "Message"
        };

        string result = entry.ToStandardFormat();

        Assert.StartsWith("05-12-2025", result);
    }
}
