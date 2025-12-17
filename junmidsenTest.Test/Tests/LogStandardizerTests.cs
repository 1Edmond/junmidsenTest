using junmidsenTest.Tasks.Third_Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junmidsenTest.Test.Tests;

public class LogStandardizerTests : IDisposable
{
    private const string TestInputFile = "test_input.log";
    private const string TestOutputFile = "test_output.log";
    private const string TestProblemsFile = "test_problems.txt";

    public void Dispose()
    {
        if (File.Exists(TestInputFile)) File.Delete(TestInputFile);
        if (File.Exists(TestOutputFile)) File.Delete(TestOutputFile);
        if (File.Exists(TestProblemsFile)) File.Delete(TestProblemsFile);
    }

    [Fact]
    public void ProcessLogFile_ValidFormat1Entries_CreatesCorrectOutput()
    {
        var inputLines = new[]
        {
                "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'",
                "15.03.2025 10:30:22.100 WARNING Mémoire faible"
            };
        File.WriteAllLines(TestInputFile, inputLines, Encoding.UTF8);

        var standardizer = new LogStandardizer(TestOutputFile, TestProblemsFile);

        var stats = standardizer.ProcessLogFile(TestInputFile);

        Assert.Equal(2, stats.SuccessCount);
        Assert.Equal(0, stats.ErrorCount);
        Assert.True(File.Exists(TestOutputFile));

        string[] outputLines = File.ReadAllLines(TestOutputFile, Encoding.UTF8);
        Assert.Equal(2, outputLines.Length);
        Assert.Contains("10-03-2025", outputLines[0]);
        Assert.Contains("INFO", outputLines[0]);
        Assert.Contains("DEFAULT", outputLines[0]);
    }

    [Fact]
    public void ProcessLogFile_ValidFormat2Entries_CreatesCorrectOutput()
    {
        var inputLines = new[]
        {
                "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO'"
            };
        File.WriteAllLines(TestInputFile, inputLines, Encoding.UTF8);

        var standardizer = new LogStandardizer(TestOutputFile, TestProblemsFile);

        var stats = standardizer.ProcessLogFile(TestInputFile);

        Assert.Equal(1, stats.SuccessCount);
        Assert.Equal(0, stats.ErrorCount);

        string[] outputLines = File.ReadAllLines(TestOutputFile, Encoding.UTF8);
        Assert.Contains("10-03-2025", outputLines[0]);
        Assert.Contains("INFO", outputLines[0]);
        Assert.Contains("MobileComputer.GetDeviceId", outputLines[0]);
    }

    [Fact]
    public void ProcessLogFile_MixedValidFormats_ProcessesAllCorrectly()
    {
        var inputLines = new[]
        {
                "10.03.2025 15:14:49.523 INFORMATION Message 1",
                "2025-03-10 15:14:51.5882| INFO|11|Method| Message 2",
                "15.03.2025 10:30:22.100 WARNING Message 3",
                "2025-03-15 10:30:25.2341| ERROR|5|Database.Connect| Message 4"
            };
        File.WriteAllLines(TestInputFile, inputLines, Encoding.UTF8);

        var standardizer = new LogStandardizer(TestOutputFile, TestProblemsFile);

        // Act
        var stats = standardizer.ProcessLogFile(TestInputFile);

        // Assert
        Assert.Equal(4, stats.SuccessCount);
        Assert.Equal(0, stats.ErrorCount);

        string[] outputLines = File.ReadAllLines(TestOutputFile, Encoding.UTF8);
        Assert.Equal(4, outputLines.Length);
    }

    [Fact]
    public void ProcessLogFile_InvalidEntries_WritesToProblemsFile()
    {
        // Arrange
        var inputLines = new[]
        {
                "10.03.2025 15:14:49.523 INFORMATION Valid message",
                "This is an invalid log line",
                "Another invalid line"
            };
        File.WriteAllLines(TestInputFile, inputLines, Encoding.UTF8);

        var standardizer = new LogStandardizer(TestOutputFile, TestProblemsFile);

        var stats = standardizer.ProcessLogFile(TestInputFile);

        Assert.Equal(1, stats.SuccessCount);
        Assert.Equal(2, stats.ErrorCount);
        Assert.True(File.Exists(TestProblemsFile));

        string[] problemLines = File.ReadAllLines(TestProblemsFile, Encoding.UTF8);
        Assert.Equal(2, problemLines.Length);
        Assert.Contains("This is an invalid log line", problemLines);
    }

    [Fact]
    public void ProcessLogFile_EmptyFile_ReturnsZeroStats()
    {
        File.WriteAllText(TestInputFile, string.Empty);
        var standardizer = new LogStandardizer(TestOutputFile, TestProblemsFile);

        var stats = standardizer.ProcessLogFile(TestInputFile);

        Assert.Equal(0, stats.SuccessCount);
        Assert.Equal(0, stats.ErrorCount);
    }

    [Fact]
    public void ProcessLogFile_NonExistentFile_ThrowsFileNotFoundException()
    {
        var standardizer = new LogStandardizer(TestOutputFile, TestProblemsFile);

        Assert.Throws<FileNotFoundException>(() =>
            standardizer.ProcessLogFile("nonexistent.log"));
    }

    [Fact]
    public void ProcessLogFile_LogLevelNormalization_WorksCorrectly()
    {
        var inputLines = new[]
        {
                "10.03.2025 15:14:49.523 INFORMATION Message",
                "10.03.2025 15:14:50.523 WARNING Message",
                "10.03.2025 15:14:51.523 ERROR Message",
                "10.03.2025 15:14:52.523 DEBUG Message"
            };
        File.WriteAllLines(TestInputFile, inputLines, Encoding.UTF8);

        var standardizer = new LogStandardizer(TestOutputFile, TestProblemsFile);

        standardizer.ProcessLogFile(TestInputFile);

        string[] outputLines = File.ReadAllLines(TestOutputFile, Encoding.UTF8);
        Assert.Contains("INFO", outputLines[0]);
        Assert.Contains("WARN", outputLines[1]);
        Assert.Contains("ERROR", outputLines[2]);
        Assert.Contains("DEBUG", outputLines[3]);
    }

    [Fact]
    public void ProcessLogFile_PreservesOriginalTimeFormat()
    {
        var inputLines = new[]
        {
                "10.03.2025 15:14:49.523 INFO Message",
                "2025-03-10 15:14:51.5882| INFO|11|Method| Message"
            };
        File.WriteAllLines(TestInputFile, inputLines, Encoding.UTF8);

        var standardizer = new LogStandardizer(TestOutputFile, TestProblemsFile);

        standardizer.ProcessLogFile(TestInputFile);

        string[] outputLines = File.ReadAllLines(TestOutputFile, Encoding.UTF8);
        Assert.Contains("15:14:49.523", outputLines[0]);
        Assert.Contains("15:14:51.5882", outputLines[1]);
    }

    [Fact]
    public void ProcessLogFile_DefaultMethodWhenMissing()
    {
        var inputLines = new[]
        {
                "10.03.2025 15:14:49.523 INFO Message without method"
            };
        File.WriteAllLines(TestInputFile, inputLines, Encoding.UTF8);

        var standardizer = new LogStandardizer(TestOutputFile, TestProblemsFile);

        standardizer.ProcessLogFile(TestInputFile);

        string output = File.ReadAllText(TestOutputFile, Encoding.UTF8);
        Assert.Contains("DEFAULT", output);
    }

    [Fact]
    public void ProcessLogFile_PreservesUnicodeCharacters()
    {
        var inputLines = new[]
        {
                "10.03.2025 15:14:49.523 INFO Версия программы: '3.4.0.48729'",
                "2025-03-10 15:14:51.5882| INFO|11|Method| Код устройства: 'Тест'"
            };
        File.WriteAllLines(TestInputFile, inputLines, Encoding.UTF8);

        var standardizer = new LogStandardizer(TestOutputFile, TestProblemsFile);

        standardizer.ProcessLogFile(TestInputFile);

        string[] outputLines = File.ReadAllLines(TestOutputFile, Encoding.UTF8);
        Assert.Contains("Версия программы", outputLines[0]);
        Assert.Contains("Код устройства", outputLines[1]);
    }
}
