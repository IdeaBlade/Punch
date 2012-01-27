//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

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
