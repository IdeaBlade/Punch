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
using Cocktail.Tests.Helpers;

#if !NETFX_CORE
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using System.Composition;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Cocktail.Tests
{
    [TestClass]
    public class BusyWatcherUnitTests : CocktailTestBase
    {
        [TestMethod]
        public void ShouldBeBusyInsideScope()
        {
            var busyWatcher = new BusyWatcher();
            using (busyWatcher.GetTicket())
            {
                Assert.IsTrue(busyWatcher.IsBusy);
            }

            Assert.IsFalse(busyWatcher.IsBusy);
        }

        [TestMethod]
        public void ShouldNotifyOfPropertyChanged()
        {
            int propertyChangedCount = 0;
            var busyWatcher = new BusyWatcher();
            busyWatcher.PropertyChanged += (sender, args) => propertyChangedCount++;

            using (busyWatcher.GetTicket())
            {
                Assert.IsTrue(propertyChangedCount == 1);
            }

            Assert.IsTrue(propertyChangedCount == 2);
        }

        [TestMethod]
        public void ShouldIncrementDecrementBusyCounter()
        {
            var busyWatcher = new BusyWatcher();
            busyWatcher.AddWatch();
            Assert.IsTrue(busyWatcher.IsBusy);

            busyWatcher.RemoveWatch();
            Assert.IsFalse(busyWatcher.IsBusy);
        }

        [TestMethod]
        public void ShouldNestBusyState()
        {
            var busyWatcher = new BusyWatcher();

            busyWatcher.AddWatch();
            Assert.IsTrue(busyWatcher.IsBusy);

            using (busyWatcher.GetTicket())
            {
                Assert.IsTrue(busyWatcher.IsBusy);
            }

            Assert.IsTrue(busyWatcher.IsBusy);
            busyWatcher.RemoveWatch();
            Assert.IsFalse(busyWatcher.IsBusy);
        }

        [TestMethod]
        public void ShoultThrowInvalidOperationException()
        {
            var expectedExceptionThrown = false;
            try
            {
                var busyWatcher = new BusyWatcher();
                busyWatcher.RemoveWatch();
            }
            catch (InvalidOperationException)
            {
                expectedExceptionThrown = true;
            }
            Assert.IsTrue(expectedExceptionThrown);
        }
    }
}