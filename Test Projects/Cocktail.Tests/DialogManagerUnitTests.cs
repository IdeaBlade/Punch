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
using System.Linq;
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
            var operation = _dialogManager.ShowMessageAsync("Test", DialogButtons.OkCancel);

            Assert.IsTrue(operation.DialogResult == DialogResult.Cancel);
            Assert.IsTrue(operation.Cancelled);
            Assert.IsTrue(operation.IsCompleted);
        }

        [TestMethod]
        public void ShouldNotCancel()
        {
            TestWindowManager.Instance.TestDialogResult = DialogResult.Ok;
            var operation = _dialogManager.ShowMessageAsync("Test", DialogButtons.OkCancel);

            Assert.IsTrue(operation.DialogResult == DialogResult.Ok);
            Assert.IsFalse(operation.Cancelled);
            Assert.IsTrue(operation.IsCompleted);
        }

        [TestMethod]
        public void ShouldInvokeCustomCommand()
        {
            var invokeCount = 0;

            TestWindowManager.Instance.TestDialogResult = DialogResult.Ok;
            var command = new DialogUICommand<DialogResult>(DialogResult.Ok);

            command.Invoked += (sender, args) => invokeCount++;
            ((IUICommand) command).Invoked += (sender, args) => invokeCount++;
            var operation = _dialogManager.ShowMessageAsync(new[] {command}, "Test");

            Assert.IsTrue(invokeCount == 2);
            Assert.IsTrue(operation.DialogResult == DialogResult.Ok);
            Assert.IsFalse(operation.Cancelled);
            Assert.IsTrue(operation.IsCompleted);
        }

        [TestMethod]
        public void ShouldCancelCommand()
        {
            var cmd1 = new DialogUICommand<DialogResult>(DialogResult.Ok);
            cmd1.Invoked += (sender, args) => args.Cancel();
            var button1 = new DialogButton(cmd1, new DialogHostBase());

            Assert.IsFalse(button1.InvokeCommand());
            Assert.IsTrue(cmd1.WasCancelled);

            IUICommand cmd2 = new DialogUICommand<DialogResult>(DialogResult.Ok);
            cmd2.Invoked += (sender, args) => args.Cancel();
            var button2 = new DialogButton(cmd2, new DialogHostBase());

            Assert.IsFalse(button2.InvokeCommand());
            Assert.IsTrue(cmd2.WasCancelled);
        }

        [TestMethod]
        public void ShouldUseCustomCancel()
        {
            TestWindowManager.Instance.TestDialogResult = "Cancel";
            var operation = _dialogManager.ShowMessageAsync("Test", null, "Cancel", new[] { "Ok", "Cancel" });

            Assert.IsTrue(operation.DialogResult == "Cancel");
            Assert.IsTrue(operation.Cancelled);
            Assert.IsTrue(operation.IsCompleted);
        }

        [TestMethod]
        public void ShouldNotUseCustomCancel()
        {
            TestWindowManager.Instance.TestDialogResult = "Cancel";
            var operation = _dialogManager.ShowMessageAsync("Test", null, null, new[] { "Ok", "Cancel" });

            Assert.IsTrue(operation.DialogResult == "Cancel");
            Assert.IsFalse(operation.Cancelled);
            Assert.IsTrue(operation.IsCompleted);
        }

        [TestMethod]
        public void ShouldBeIncomplete()
        {
            var operation = new ShowDialogResult<object>(null, null);

            Assert.IsNull(operation.DialogResult);
            Assert.IsFalse(operation.Cancelled);
            Assert.IsFalse(operation.IsCompleted);
        }

        [TestMethod]
        public void ShouldNotCancelTask()
        {
            TestWindowManager.Instance.TestDialogResult = DialogResult.Ok;
            var task = _dialogManager.ShowMessageAsync("Test", DialogButtons.OkCancel).AsTask();

            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.Result == DialogResult.Ok);
            Assert.IsFalse(task.IsCanceled);
        }

        [TestMethod]
        public void ShouldCancelTask()
        {
            TestWindowManager.Instance.TestDialogResult = DialogResult.Cancel;
            var task = _dialogManager.ShowMessageAsync("Test", DialogButtons.OkCancel).AsTask();

            Assert.IsTrue(task.IsCanceled);
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

                var dialogHost = (DialogHostBase)rootModel;
                try
                {
                    var button = dialogHost.DialogButtons.First(x => x.DialogResult.Equals(TestDialogResult));
                    ((DialogHostBase)rootModel).Close(button);
                }
                catch (NotSupportedException)
                {
                    // Close will throw an expected exception, because it can't actually close the VM without a parent or a view.
                }

                // Simulating closing.
                ((IDeactivate) rootModel).Deactivate(true);

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