// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Messages;
using MefContrib.Hosting.Interception;
using MefContrib.Hosting.Interception.Configuration;

#if SILVERLIGHT
using System.Windows;
#else
using System.Windows.Threading;
#endif

namespace Common
{
    public class BootstrapperBase<T> : CocktailMefBootstrapper<T>, IExportedValueInterceptor
    {
        // Automatically instantiate and hold all discovered MessageProcessors
        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        public IEnumerable<IMessageProcessor> MessageProcessors { get; set; }

        [Import]
        public IErrorHandler ErrorHandler { get; set; }

        #region IExportedValueInterceptor Members

        object IExportedValueInterceptor.Intercept(object value)
        {
            SubscribeToEventAggregator(value);
            return value;
        }

        #endregion

#if SILVERLIGHT
        protected override void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            ErrorHandler.HandleError(e.ExceptionObject);
            e.Handled = true;
        }
#else
        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorHandler.HandleError(e.Exception);
            e.Handled = true;
        }
#endif

        protected override void BuildUp(object instance)
        {
            base.BuildUp(instance);
            SubscribeToEventAggregator(instance);
        }

#if !SILVERLIGHT
        protected override void StartRuntime()
        {
            base.StartRuntime();

            // Enable asynchronous navigation for all navigation properties in every client EntityManager.
            EntityManager.EntityManagerCreated +=
                (sender, args) =>
                    {
                        if (args.EntityManager.IsClient)
                            args.EntityManager.DefaultEntityReferenceStrategy = EntityReferenceStrategy.DefaultAsync;
                    };
        }
#endif

        // Use InterceptingCatalog from MefContrib to centrally handle EventAggregator subscriptions.
        protected override ComposablePartCatalog PrepareCompositionCatalog()
        {
            var cfg = new InterceptionConfiguration().AddInterceptor(this);
            return new InterceptingCatalog(base.PrepareCompositionCatalog(), cfg);
        }

        private void SubscribeToEventAggregator(object instance)
        {
            if (instance is IHandle)
            {
                LogFns.DebugWriteLine(string.Format("Automatically subscribing instance of {0} to EventAggregator.",
                                                    instance.GetType().Name));
                EventFns.Subscribe(instance);
            }
        }
    }
}