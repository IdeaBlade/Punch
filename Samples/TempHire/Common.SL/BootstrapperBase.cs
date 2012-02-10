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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Windows;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Messages;
using MefContrib.Hosting.Interception;
using MefContrib.Hosting.Interception.Configuration;

namespace Common
{
    public class BootstrapperBase<T> : FrameworkBootstrapper<T>, IExportedValueInterceptor
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

        protected override void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            ErrorHandler.HandleError(e.ExceptionObject);
            e.Handled = true;
        }

        protected override void BuildUp(object instance)
        {
            base.BuildUp(instance);
            SubscribeToEventAggregator(instance);
        }

        // Use InterceptingCatalog from MefContrib to centrally handle EventAggregator subscriptions.
        protected override ComposablePartCatalog PrepareCompositionCatalog()
        {
            InterceptionConfiguration cfg = new InterceptionConfiguration().AddInterceptor(this);
            return new InterceptingCatalog(Composition.Catalog, cfg);
        }

        private void SubscribeToEventAggregator(object instance)
        {
            if (instance is IHandle)
            {
                LogFns.DebugWriteLine(string.Format("Automatically subscribing instance of {0} to EventAggregator.", instance.GetType().Name));
                EventFns.Subscribe(instance);                
            }
        }
    }
}