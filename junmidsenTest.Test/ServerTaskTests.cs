using junmidsenTest.SecondTask.Tasks;

namespace junmidsenTest.Test;

public class ServerTaskTests
{
    [Fact]
    public void GetCount_InitialValue_ReturnsZero()
    {
        ServerTask.Reset();

        int result = ServerTask.GetCount();

        Assert.Equal(0, result);
    }

    [Fact]
    public void AddToCount_SingleValue_UpdatesCorrectly()
    {
        ServerTask.Reset();

        ServerTask.AddToCount(10);
        int result = ServerTask.GetCount();

        Assert.Equal(10, result);
    }

    [Fact]
    public void AddToCount_MultipleValues_SumsCorrectly()
    {
        ServerTask.Reset();

        ServerTask.AddToCount(5);
        ServerTask.AddToCount(10);
        ServerTask.AddToCount(15);
        int result = ServerTask.GetCount();

        Assert.Equal(30, result);
    }

    [Fact]
    public void GetCount_MultipleReaders_CanReadSimultaneously()
    {
        ServerTask.Reset();
        ServerTask.AddToCount(100);
        var tasks = new List<Task<int>>();
        int readerCount = 20;

        for (int i = 0; i < readerCount; i++)
        {
            tasks.Add(Task.Run(() => ServerTask.GetCount()));
        }

        Task.WaitAll(tasks.ToArray());

        Assert.All(tasks, task => Assert.Equal(100, task.Result));
    }

    [Fact]
    public void AddToCount_ConcurrentWriters_AllWritesApplied()
    {
        ServerTask.Reset();
        var tasks = new List<Task>();
        int writerCount = 100;

        for (int i = 0; i < writerCount; i++)
        {
            tasks.Add(Task.Run(() => ServerTask.AddToCount(1)));
        }

        Task.WaitAll(tasks.ToArray());

        int result = ServerTask.GetCount();
        Assert.Equal(writerCount, result);
    }

    [Fact]
    public void AddToCount_ConcurrentWritersWithLargeValues_ThreadSafe()
    {
        ServerTask.Reset();
        var tasks = new List<Task>();
        int writerCount = 50;
        int valuePerWriter = 100;

        for (int i = 0; i < writerCount; i++)
        {
            tasks.Add(Task.Run(() => ServerTask.AddToCount(valuePerWriter)));
        }

        Task.WaitAll(tasks.ToArray());

        int expected = writerCount * valuePerWriter;
        int actual = ServerTask.GetCount();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetCount_DuringWrite_BlocksUntilWriteCompletes()
    {
        ServerTask.Reset();
        bool writerStarted = false;
        bool writerCompleted = false;
        int readerValue = -1;

        var writerTask = Task.Run(() =>
        {
            writerStarted = true;
            ServerTask.AddToCount(50);
            Thread.Sleep(100);
            writerCompleted = true;
        });

        while (!writerStarted) Thread.Sleep(1);

        var readerTask = Task.Run(() =>
        {
            readerValue = ServerTask.GetCount();
        });

        Task.WaitAll(writerTask, readerTask);

        Assert.True(writerCompleted);
        Assert.Equal(50, readerValue);
    }

    [Fact]
    public void Server_MixedReadersAndWriters_ThreadSafe()
    {
        ServerTask.Reset();
        var tasks = new List<Task>();
        int totalOperations = 1000;
        int writePercentage = 30; 

        var random = new Random(42);
        for (int i = 0; i < totalOperations; i++)
        {
            if (random.Next(100) < writePercentage)
            {
                tasks.Add(Task.Run(() => ServerTask.AddToCount(1)));
            }
            else
            {
                tasks.Add(Task.Run(() => ServerTask.GetCount()));
            }
        }

        Task.WaitAll(tasks.ToArray());

        int finalValue = ServerTask.GetCount();
        int expectedWrites = totalOperations * writePercentage / 100;

        Assert.InRange(finalValue, expectedWrites - 50, expectedWrites + 50);
    }

    [Fact]
    public void Server_StressTest_10000Operations_ThreadSafe()
    {
        ServerTask.Reset();
        var tasks = new List<Task>();
        int iterations = 10000;
        int writeCount = 0;

        for (int i = 0; i < iterations; i++)
        {
            if (i % 10 == 0)
            {
                Interlocked.Increment(ref writeCount);
                tasks.Add(Task.Run(() => ServerTask.AddToCount(1)));
            }
            else
            {
                tasks.Add(Task.Run(() => ServerTask.GetCount()));
            }
        }

        Task.WaitAll(tasks.ToArray());

        int expectedValue = writeCount;
        int actualValue = ServerTask.GetCount();
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Reset_AfterOperations_ResetsToZero()
    {
        ServerTask.AddToCount(100);
        ServerTask.AddToCount(50);

        ServerTask.Reset();
        int result = ServerTask.GetCount();

        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(-50)]
    [InlineData(0)]
    public void AddToCount_VariousValues_UpdatesCorrectly(int value)
    {
        ServerTask.Reset();
        int initialValue = ServerTask.GetCount();

        ServerTask.AddToCount(value);
        int result = ServerTask.GetCount();

        Assert.Equal(initialValue + value, result);
    }

    [Fact]
    public void Server_ConcurrentReadsDuringWrite_NoDataCorruption()
    {
        ServerTask.Reset();
        var readValues = new List<int>();
        var lockObj = new object();

        var writerTask = Task.Run(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                ServerTask.AddToCount(10);
                Thread.Sleep(10);
            }
        });

        var readerTasks = new List<Task>();
        for (int i = 0; i < 50; i++)
        {
            readerTasks.Add(Task.Run(() =>
            {
                int value = ServerTask.GetCount();
                lock (lockObj)
                {
                    readValues.Add(value);
                }
            }));
        }

        Task.WaitAll(new[] { writerTask }.Concat(readerTasks).ToArray());

        Assert.All(readValues, value => Assert.Equal(0, value % 10));
        Assert.Equal(100, ServerTask.GetCount());
    }



}
