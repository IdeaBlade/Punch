namespace Common.BusyWatcher
{
    public interface IBusyWatcher
    {
        bool IsBusy { get; }

        BusyWatcher.BusyWatcherTicket GetTicket();

        void AddWatch();
        void RemoveWatch();
    }
}