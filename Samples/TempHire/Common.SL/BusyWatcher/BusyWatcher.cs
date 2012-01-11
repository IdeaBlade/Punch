using System.ComponentModel.Composition;
using System.Threading;
using Caliburn.Micro;
using System;

namespace Common.BusyWatcher
{
    [Export(typeof(IBusyWatcher))]
    public class BusyWatcher : PropertyChangedBase, IBusyWatcher
    {
        int _counter;

        public bool IsBusy
        {
            get
            {
                return _counter > 0;
            }
        }

        public BusyWatcherTicket GetTicket()
        {
            return new BusyWatcherTicket(this);
        }

        public void AddWatch()
        {
            if (Interlocked.Increment(ref _counter) == 1)
            {
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        public void RemoveWatch()
        {
            if (Interlocked.Decrement(ref _counter) == 0)
            {
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        public class BusyWatcherTicket : IDisposable
        {
            readonly IBusyWatcher _parent;

            public BusyWatcherTicket(IBusyWatcher parent)
            {
                _parent = parent;
                _parent.AddWatch();
            }

            public void Dispose()
            {
                _parent.RemoveWatch();
            }
        }

    }
}
