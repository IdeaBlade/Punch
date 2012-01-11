using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Cocktail;
using Common.Errors;
using Common.Messages;
using DomainModel;
using TempHire.Authentication;

namespace TempHire
{
    public class AppBootstrapper : FrameworkBootstrapper<HarnessViewModel>
    {
        static AppBootstrapper()
        {
            UsesFakeStore<TempHireEntities>();
        }

        // Automatically instantiate and hold all discovered MessageProcessors
        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        public IEnumerable<IMessageProcessor> MessageProcessors { get; set; }

        [Import]
        public IErrorHandler ErrorHandler { get; set; }

        protected override void InitializeCompositionBatch(CompositionBatch batch)
        {
            base.InitializeCompositionBatch(batch);

            batch.AddExportedValue<IAuthenticationService>(new FakeAuthenticationService());
        }

        protected override void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            ErrorHandler.HandleError(e.ExceptionObject);
            e.Handled = true;
        }
    }
}