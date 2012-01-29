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

using System;
using System.Threading;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>A service to manage a busy indicator</summary>
    /// <example>
    /// 	<code title="Example 1" description="Demonstrates using the BusyWatcher in a ViewModel." lang="C#">
    /// public class LoginViewModel
    /// {
    ///     public LoginViewModel()
    ///     {
    ///         Busy = new BusyWatcher();
    ///     }
    ///  
    ///     public IBusyWatcher Busy { get; private set; }
    ///  
    ///     public IEnumerable&lt;IResult&gt; Login()
    ///     {
    ///         using (Busy.GetTicket())
    ///         {
    ///             // Snip .. removed for clarity
    ///             yield return _authenticationService.LoginAsync(credential, onFail: e =&gt; FailureMessage = e.Message);
    ///  
    ///             if (_authenticationService.IsLoggedIn)
    ///                 TryClose();
    ///         }
    ///     }
    /// }</code>
    /// 	<code title="Example 2" description="Demonstrates binding to the BusyWatcher." lang="XAML">
    /// &lt;toolkit:BusyIndicator BorderBrush="#FF1D5380" IsBusy="{Binding Busy.IsBusy}"&gt;
    ///     &lt;toolkit:BusyIndicator.BusyContent&gt;
    ///         &lt;TextBlock Text="Please wait" /&gt;
    ///     &lt;/toolkit:BusyIndicator.BusyContent&gt;
    ///  
    /// &lt;/toolkit:BusyIndicator&gt;
    /// </code>
    /// </example>
    public class BusyWatcher : PropertyChangedBase, IBusyWatcher
    {
        private int _counter;

        #region IBusyWatcher Members

        /// <summary>
        /// Returns true if the state of the BusyWatcher is currently busy. This property can be directly bound
        /// to a busy indicator control in XAML.
        /// </summary>
        /// <remarks>IsBusy is true as long as the internal busy counter is greater than zero.</remarks>
        public bool IsBusy
        {
            get { return _counter > 0; }
        }

        /// <summary>
        /// Returns a disposable ticket to manage busy state around a using() scope.
        /// </summary>
        /// <returns>Ticket implementing IDisposable.</returns>
        /// <remarks>
        /// The internal busy indicator is incremented by calling this method and as soon as the Dispose()
        /// method is called on the ticket, the internal busy counter is decremented.
        /// </remarks>
        public BusyWatcherTicket GetTicket()
        {
            return new BusyWatcherTicket(this);
        }

        /// <summary>
        /// Increments the internal busy counter.
        /// </summary>
        public void AddWatch()
        {
            if (Interlocked.Increment(ref _counter) == 1)
            {
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        /// <summary>
        /// Decrements the internal busy counter.
        /// </summary>
        public void RemoveWatch()
        {
            if (Interlocked.Decrement(ref _counter) == 0)
            {
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        #endregion

        #region Nested type: BusyWatcherTicket

        /// <summary>A disposable ticket controlling busy state around a scope.</summary>
        public class BusyWatcherTicket : IDisposable
        {
            private readonly IBusyWatcher _parent;

            /// <summary>
            /// Initializes a new BusyWatcherTicket instance.
            /// </summary>
            /// <param name="parent">The BusyWatcher owning the ticket.</param>
            public BusyWatcherTicket(IBusyWatcher parent)
            {
                _parent = parent;
                _parent.AddWatch();
            }

            #region IDisposable Members

            /// <summary>
            /// Decrements the internal busy counter of the associated busy watcher.
            /// </summary>
            /// <remarks>Do not call more than once per ticket.</remarks>
            public void Dispose()
            {
                _parent.RemoveWatch();
            }

            #endregion
        }

        #endregion
    }
}