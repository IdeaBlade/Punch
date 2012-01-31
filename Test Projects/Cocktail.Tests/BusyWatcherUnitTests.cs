using Cocktail.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}