namespace junmidsenTest.SecondTask.Tasks;

public static class ServerTask
{
    private static int _count = 0;
    private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();


    public static int GetCount()
    {
        _lock.EnterReadLock();
        try
        {
            return _count;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public static void AddToCount(int value)
    {
        _lock.EnterWriteLock();
        try
        {
            _count += value;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public static void Reset()
    {
        _lock.EnterWriteLock();
        try
        {
            _count = 0;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public static (int WaitingReaders, int WaitingWriters, bool IsReadLockHeld, bool IsWriteLockHeld) GetLockStatus()
    {
        return (
            _lock.WaitingReadCount,
            _lock.WaitingWriteCount,
            _lock.IsReadLockHeld,
            _lock.IsWriteLockHeld
        );
    }

}
