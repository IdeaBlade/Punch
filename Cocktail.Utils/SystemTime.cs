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

namespace Cocktail
{
    /// <summary>
    /// Get current DateTime for the system
    /// </summary>
    /// <remarks>
    /// Supports testing of time-sensitive methods by enabling you to
    /// control the test-time <see cref="System.DateTime"/> value
    /// via the <see cref="NowFunction"/> function.
    /// <para>
    /// The testability of your application improves when you use SystemTime 
    /// where you would conventionally call on <see cref="System.DateTime"/>.
    /// </para>
    /// </remarks>
    public static class SystemTime
    {
        /// <summary>
        /// Get or set the function that purports to tell the current DateTime on this machine
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DateTime.Now"/> but you can
        /// replace it with a different function during testing in order to pretend
        /// that machine time is different.
        /// Useful for machine time sensitive tests.
        /// </remarks>
        public static Func<DateTime> NowFunction = () => DateTime.Now;

        /// <summary>
        /// Get a <see cref="System.DateTime"/> object that is set to the
        /// current date and time on this computer, expressed in local time.
        /// </summary>
        /// <remarks>
        /// Same semantics as <see cref="System.DateTime.Now"/>
        /// </remarks>
        public static DateTime Now { get { return NowFunction(); } }

        /// <summary>
        /// Get a <see cref="System.DateTime"/> object that is set to the
        /// current date and time on this computer, 
        /// expressed in Coordinated Universal Time.
        /// </summary>
        /// <remarks>
        /// Same semantics as <see cref="System.DateTime.UtcNow"/>
        /// </remarks> 
        public static DateTime UtcNow { get { return NowFunction().ToUniversalTime(); } }

        /// <summary>
        /// Gets the Date component of this instance
        /// </summary>
        /// <remarks>
        /// Same semantics as <see cref="System.DateTime.Today"/>
        /// </remarks>        
        public static DateTime Today { get { return NowFunction().Date; } }

        /// <summary>
        /// Restore <see cref="NowFunction"/> to return the
        /// current <see cref="System.DateTime"/> on this machine.
        /// </summary>
        public static void Reset()
        {
            NowFunction = () => DateTime.Now;
        }
    }
}