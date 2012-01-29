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