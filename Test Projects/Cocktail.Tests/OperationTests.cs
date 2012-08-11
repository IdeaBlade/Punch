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
using System.Threading.Tasks;
using Cocktail.Tests.Helpers;
using IdeaBlade.EntityModel;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cocktail.Tests
{
    [TestClass]
    public class OperationTests : CocktailTestBase
    {
        [TestMethod]
        public void ShouldAlwaysCompleteSuccessfully()
        {
            var operation = OperationResult.FromResult(true);
            Assert.IsTrue(operation.CompletedSuccessfully);
            Assert.IsFalse(operation.HasError);
            Assert.IsNull(operation.Error);
            Assert.IsFalse(operation.Cancelled);
            Assert.IsFalse(operation.IsErrorHandled);
            Assert.IsTrue(operation.IsCompleted);
        }

        [TestMethod]
        public void ShouldMarkErrorAsHandled()
        {
            var operation = new CompleteWithError().AsOperationResult().ContinueOnError();

            Assert.IsFalse(operation.CompletedSuccessfully);
            Assert.IsTrue(operation.HasError);
            Assert.IsNotNull(operation.Error);
            Assert.IsFalse(operation.Cancelled);
            Assert.IsTrue(operation.IsErrorHandled);
            Assert.IsTrue(operation.IsCompleted);
        }

        [TestMethod]
        public void ShouldCancel()
        {
            var operation = new CancelledOperation().AsOperationResult();

            Assert.IsFalse(operation.CompletedSuccessfully);
            Assert.IsFalse(operation.HasError);
            Assert.IsNull(operation.Error);
            Assert.IsTrue(operation.Cancelled);
            Assert.IsFalse(operation.IsErrorHandled);
            Assert.IsTrue(operation.IsCompleted);
        }

        [TestMethod]
        public void ShouldBeIncomplete()
        {
            var operation = new IncompleteOperation().AsOperationResult();

            Assert.IsFalse(operation.CompletedSuccessfully);
            Assert.IsFalse(operation.HasError);
            Assert.IsNull(operation.Error);
            Assert.IsFalse(operation.Cancelled);
            Assert.IsFalse(operation.IsErrorHandled);
            Assert.IsFalse(operation.IsCompleted);
        }

        [TestMethod]
        public void ShouldNotWrapExistingOperationResult()
        {
            var op1 = new OperationResult(AlwaysCompleted.Instance);
            var op2 = op1.AsOperationResult();
            Assert.IsTrue(ReferenceEquals(op1, op2));
        }

        [TestMethod]
        public void ShouldWrapINotifyCompleted()
        {
            var op = AlwaysCompleted.Instance.AsOperationResult();
            Assert.IsNotNull(op);
        }

        [TestMethod]
        public void ShouldCompleteTask()
        {
            var task = OperationResult.FromResult(true).AsTask();
            
            Assert.IsFalse(task.IsFaulted);
            Assert.IsFalse(task.IsCanceled);
        }

        [TestMethod]
        public void ShouldFailTaskAndMarkErrorAsHandled()
        {
            var op = new CompleteWithError().AsOperationResult();
            var task = op.AsTask();
            task.ContinueWith(t =>
                                  {
                                      Assert.IsTrue(t.IsFaulted);
                                      Assert.IsFalse(t.IsCanceled);
                                  });
            Assert.IsTrue(op.IsErrorHandled);
            Assert.IsTrue(task.IsFaulted);
        }

        //[TestMethod]
        //[Asynchronous]
        //[Timeout(10000)]
        //public void ShouldWrapTaskCompleteSuccessfully()
        //{
        //    DoItAsync(
        //        () => Task.Factory.StartNew(() => true).AsOperationResult()
        //                  .ContinueWith(op =>
        //                                    {
        //                                        Assert.IsTrue(op.CompletedSuccessfully);
        //                                        Assert.IsFalse(op.HasError);
        //                                        Assert.IsFalse(op.Cancelled);
        //                                        Assert.IsTrue(op.Result);

        //                                        TestComplete();
        //                                    }));
        //}

        //[TestMethod]
        //[Asynchronous]
        //[Timeout(10000)]
        //public void ShouldWarpTaskCompleteWithError()
        //{
        //    DoItAsync(
        //        () => Task.Factory.StartNew(() => { throw new Exception("Fault"); }).AsOperationResult()
        //                  .ContinueWith(op =>
        //                                    {
        //                                        Assert.IsFalse(op.CompletedSuccessfully);
        //                                        Assert.IsTrue(op.HasError);
        //                                        Assert.IsFalse(op.Cancelled);
        //                                        Assert.IsNotNull(op.Error);

        //                                        TestComplete();
        //                                    }));
        //}

        private class CompleteWithError : INotifyCompleted
        {
            private readonly INotifyCompletedArgs _args = new CompleteWithErrorArgs();

            public void WhenCompleted(Action<INotifyCompletedArgs> completedAction)
            {
                completedAction(_args);
            }

            private class CompleteWithErrorArgs : INotifyCompletedArgs
            {
                public CompleteWithErrorArgs()
                {
                    Error = new Exception();
                    Cancelled = false;
                }

                public Exception Error { get; private set; }
                public bool Cancelled { get; private set; }
                public bool IsErrorHandled { get; set; }
            }
        }

        private class CancelledOperation : INotifyCompleted
        {
            private readonly INotifyCompletedArgs _args = new CancelledOperationArgs();

            private class CancelledOperationArgs : INotifyCompletedArgs
            {
                public CancelledOperationArgs()
                {
                    Error = null;
                    Cancelled = true;
                }

                public Exception Error { get; private set; }
                public bool Cancelled { get; private set; }
                public bool IsErrorHandled { get; set; }
            }

            public void WhenCompleted(Action<INotifyCompletedArgs> completedAction)
            {
                completedAction(_args);
            }
        }

        private class IncompleteOperation : INotifyCompleted
        {
            public void WhenCompleted(Action<INotifyCompletedArgs> completedAction)
            {
            }
        }
    }
}