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

namespace Cocktail
{
    /// <summary>
    /// A service to manage a busy indicator
    /// </summary>
    public interface IBusyWatcher
    {
        /// <summary>
        /// Returns true if the state of the BusyWatcher is currently busy. This property can be directly bound
        /// to a busy indicator control in XAML.
        /// </summary>
        /// <remarks>IsBusy is true as long as the internal busy counter is greater than zero.</remarks>
        bool IsBusy { get; }

        /// <summary>
        /// Returns a disposable ticket to manage busy state around a using() scope.
        /// </summary>
        /// <returns>Ticket implementing IDisposable.</returns>
        /// <remarks>
        /// The internal busy indicator is incremented by calling this method and as soon as the Dispose()
        /// method is called on the ticket, the internal busy counter is decremented.
        /// </remarks>
        BusyWatcher.BusyWatcherTicket GetTicket();

        /// <summary>
        /// Increments the internal busy counter.
        /// </summary>
        void AddWatch();


        /// <summary>
        /// Decrements the internal busy counter.
        /// </summary>
        void RemoveWatch();
    }
}