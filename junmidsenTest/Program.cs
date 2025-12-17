using junmidsenTest.Tasks.Third_Task;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== Log file standardizer ===\n");


CreateSampleLogFile("input.log");

try
{
    var standardizer = new LogStandardizer("output.log", "problems.txt");
    var stats = standardizer.ProcessLogFile("input.log");

    Console.WriteLine("✓ Processing completed successfully !\n");
    Console.WriteLine($"Statistics : {stats}\n");

    Console.WriteLine("=== Output file contents (output.log) ===");
    if (File.Exists("output.log"))
    {
        Console.WriteLine(File.ReadAllText("output.log", Encoding.UTF8));
    }

    if(stats.ErrorCount == 0)
    {
        Console.WriteLine("\n=== Contents of the problem file (problems.txt) ===");
        if (File.Exists("problems.txt"))
        {
            Console.WriteLine(File.ReadAllText("problems.txt", Encoding.UTF8));
        }
    }
    else
    {
        Console.WriteLine("(No problematic entries)");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error : {ex.Message}");
}

Console.WriteLine("\n=== Press any key to exit ===");
Console.ReadKey();
       

static void CreateSampleLogFile(string path)
{
    var sampleLines = new[]
    {
                "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'",
                "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'",
                "15.03.2025 10:30:22.100 WARNING Low memory detected",
                "2025-03-15 10:30:25.2341| ERROR|5|Database.Connect| Failed to connect to the database",
                "20.03.2025 08:15:33.789 DEBUG Initializing the network module",
                "2025-03-20 08:15:40.1234| WARN|8|Network.CheckConnection| High latency: 250ms",
                "Invalid line without correct format",
                "25.03.2025 12:45:10.555 ERROR Unexpected error in the main module",
                "2025-03-25 12:45:15.9876| DEBUG|3|Logger.Initialize| Logger successfully initialized"
            };

    File.WriteAllLines(path, sampleLines, Encoding.UTF8);
    Console.WriteLine($"✓ Test file created : {path}\n");
}