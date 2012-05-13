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
using IdeaBlade.EntityModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cocktail.Tests
{
    [TestClass]
    public class OperationTests : CocktailTestBase
    {
        [TestMethod]
        public void ShouldAlwaysCompleteSuccessfully()
        {
            AlwaysCompletedOperationResult operation = AlwaysCompletedOperationResult.Instance;
            Assert.IsTrue(operation.CompletedSuccessfully);
            Assert.IsFalse(operation.HasError);
            Assert.IsNull(operation.Error);
            Assert.IsFalse(operation.Cancelled);
            Assert.IsFalse(operation.IsErrorHandled);
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
            var task = AlwaysCompletedOperationResult.Instance.AsTask();
            
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
    }
}