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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Caliburn.Micro;
using Cocktail.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cocktail.Tests
{
    [TestClass]
    public class DialogManagerUnitTests : CocktailTestBase
    {
        private IDialogManager _dialogManager;

        protected override void Context()
        {
            base.Context();
            _dialogManager = new DialogManager();
        }

        [TestMethod]
        public void ShouldCancel()
        {
            TestWindowManager.Instance.TestDialogResult = DialogResult.Cancel;
            var task = _dialogManager.ShowMessageAsync("Test", DialogButtons.OkCancel);

            Assert.IsTrue(task.IsCanceled);
            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public void ShouldNotCancel()
        {
            TestWindowManager.Instance.TestDialogResult = DialogResult.Ok;
            var task = _dialogManager.ShowMessageAsync("Test", DialogButtons.OkCancel);

            Assert.IsTrue(task.Result == DialogResult.Ok);
            Assert.IsFalse(task.IsCanceled);
            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public void ShouldUseCustomCancel()
        {
            TestWindowManager.Instance.TestDialogResult = "Cancel";
            var task = _dialogManager.ShowMessageAsync("Test", null, "Cancel", new[] { "Ok", "Cancel" });

            Assert.IsTrue(task.IsCanceled);
            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public void ShouldNotUseCustomCancel()
        {
            TestWindowManager.Instance.TestDialogResult = "Cancel";
            var task = _dialogManager.ShowMessageAsync("Test", null, null, new[] { "Ok", "Cancel" });

            Assert.IsTrue(task.Result == "Cancel");
            Assert.IsFalse(task.IsCanceled);
            Assert.IsTrue(task.IsCompleted);
        }

        protected override void PrepareCompositionContainer(CompositionBatch batch)
        {
            batch.AddExportedValue<IWindowManager>(TestWindowManager.Instance);
        }

        #region Nested type: TestWindowManager

        public class TestWindowManager : IWindowManager
        {
            private static TestWindowManager _instance;

            private TestWindowManager()
            {
            }

            public static TestWindowManager Instance
            {
                get { return _instance ?? (_instance = new TestWindowManager()); }
            }

            public object TestDialogResult { get; set; }

            #region IWindowManager Members

#if SILVERLIGHT
            public void ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
#else
            public bool? ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
#endif
            {
                ((IActivate)rootModel).Activate();

                try
                {
                    ((DialogHostBase)rootModel).Close(new DialogButton(TestDialogResult));
                }
                catch (NotSupportedException)
                {
                    // Close will throw an expected exception, because it can't actually close the VM without a parent or a view.
                }

                // Simulating closing.
                ((IDeactivate)rootModel).Deactivate(true);

#if !SILVERLIGHT
                return true;
#endif
            }

#if SILVERLIGHT
            public void ShowNotification(object rootModel, int durationInMilliseconds, object context = null, IDictionary<string, object> settings = null)
            {
                throw new NotImplementedException();
            }
#endif

#if !SILVERLIGHT
            public void ShowWindow(object rootModel, object context = null, IDictionary<string, object> settings = null)
            {
                throw new NotImplementedException();
            }
#endif

            public void ShowPopup(object rootModel, object context = null, IDictionary<string, object> settings = null)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion
    }
}