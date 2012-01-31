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
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Caliburn.Micro;
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;
using Action = System.Action;

namespace Cocktail
{
    /// <summary>
    /// Sets up a composition container and provides means to interact with the container.
    /// </summary>
    public static class Composition
    {
#if SILVERLIGHT
        private static readonly Dictionary<string, XapDownloadOperation> XapDownloadOperations =
            new Dictionary<string, XapDownloadOperation>();
#endif
        private static bool _isConfigured;
        private static CompositionContainer _container;

        /// <summary>
        /// Returns true if the CompositonHost has been configured.
        /// </summary>
        public static bool IsConfigured
        {
            get { return _isConfigured && !IsInDesignMode(); }
        }

        /// <summary>Returns the catalog in use.</summary>
        public static AggregateCatalog Catalog
        {
            get { return CompositionHost.Instance.Catalog; }
        }

        /// <summary>Returns the CompositionContainer in use.</summary>
        public static CompositionContainer Container
        {
            get { return _container ?? (_container = new CompositionContainer(Catalog)); }
        }

        /// <summary>Configures the CompositionHost.</summary>
        /// <param name="compositionBatch">
        ///     Optional changes to the <span><see cref="CompositionContainer"/></span> to include during the composition.
        /// </param>
        public static void Configure(CompositionBatch compositionBatch = null)
        {
            if (IsInDesignMode()) return;

            if (IsConfigured) return;

            CompositionBatch batch = compositionBatch ?? new CompositionBatch();

            if (!ExportExists<IEventAggregator>())
                batch.AddExportedValue<IEventAggregator>(new EventAggregator());

            if (!ExportExists<IAuthenticationProvider>())
                batch.AddExportedValue<IAuthenticationProvider>(new AuthenticationManagerProvider());

            Compose(batch);

            _isConfigured = true;
        }

        /// <summary>Executes composition on the container, including the changes in the specified <see cref="CompositionBatch"/>.</summary>
        /// <param name="compositionBatch">
        /// 	Changes to the <see cref="CompositionContainer"/> to include during the composition.
        /// </param>
        public static void Compose(CompositionBatch compositionBatch)
        {
            //CheckIfConfigured();

            if (compositionBatch == null)
                throw new ArgumentNullException("compositionBatch");

            Container.Compose(compositionBatch);
        }

        /// <summary>
        /// Resets the CompositionContainer to it's initial state.
        /// </summary>
        /// <remarks>
        /// After calling <see cref="Clear"/>, <see cref="Configure"/> must be called to configure the new CompositionContainer.
        /// </remarks>
        public static void Clear()
        {
            if (_container != null)
                _container.Dispose();
            _container = null;
            _isConfigured = false;
            ResetIsInDesignModeToDefault();
        }

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instance.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public static T GetInstance<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            CheckIfConfigured();

            var exports = GetExportsCore(typeof(T), null, requiredCreationPolicy).ToList();
            if (!exports.Any())
                throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract,
                                                  typeof(T).FullName));

            return exports.Select(e => e.Value).Cast<T>().First();
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instances.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public static IEnumerable<T> GetInstances<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            CheckIfConfigured();

            var exports = GetExportsCore(typeof(T), null, requiredCreationPolicy);
            return exports.Select(e => e.Value).Cast<T>();
        }

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type or contract name.</para>
        /// </summary>
        /// <param name="serviceType">The type of the requested instance.</param>
        /// <param name="key">The contract name of the instance requested. If no contract name is specifed, the type will be used.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public static object GetInstance(Type serviceType, string key, CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            CheckIfConfigured();

            var exports = GetExportsCore(serviceType, key, requiredCreationPolicy).ToList();
            if (!exports.Any())
                throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract,
                                                  serviceType != null ? serviceType.ToString() : key));

            return exports.First().Value;
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <param name="serviceType">Type of the requested instances.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public static IEnumerable<object> GetInstances(Type serviceType, CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            if (IsInDesignMode()) return new object[0];
            CheckIfConfigured();
            IEnumerable<Export> exports = GetExportsCore(serviceType, null, requiredCreationPolicy);
            return exports.Select(e => e.Value);
        }

        /// <summary>Satisfies all the imports on the provided instance.</summary>
        /// <param name="instance">The instance for which to satisfy the MEF imports.</param>
        public static void BuildUp(object instance)
        {
            if (IsInDesignMode()) return;
            CheckIfConfigured();
            Container.SatisfyImportsOnce(instance);
        }

        /// <summary>
        /// Specifies that the provided EntityManager type makes use of the DevForce Fake Backing Store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void UsesFakeStore<T>() where T : EntityManager
        {
            if (Execute.InDesignMode)
            {
                // Must be called before the first EM gets created
                // This allows sample data to be deserialzied from a cache file at design time
                IdeaBladeConfig.Instance.ProbeAssemblyNames.Add(typeof(T).Assembly.FullName);
            }

            FakeBackingStoreManager.Instance.Register<T>();
        }

        /// <summary>
        /// Raised when the composition container is modified after initialization.
        /// </summary>
        public static event EventHandler<RecomposedEventArgs> Recomposed
        {
            add { CompositionHost.Recomposed += value; }
            remove { CompositionHost.Recomposed -= value; }
        }

#if SILVERLIGHT

        /// <summary>Asynchronously downloads a XAP file and adds all exported parts to the catalog.</summary>
        /// <param name="relativeUri">The relative URI for the XAP file to be downloaded.</param>
        /// <param name="onSuccess">User callback to be called when operation completes successfully.</param>
        /// <param name="onFail">User callback to be called when operation completes with an error.</param>
        /// <returns>Returns a handle to the download operation.</returns>
        public static OperationResult AddXap(string relativeUri, Action onSuccess = null, Action<Exception> onFail = null)
        {
            XapDownloadOperation operation;
            if (XapDownloadOperations.TryGetValue(relativeUri, out operation) && !operation.HasError) 
                return operation.AsOperationResult();

            var op = XapDownloadOperations[relativeUri] = new XapDownloadOperation(relativeUri);
            op.WhenCompleted(
                args =>
                {
                    if (args.Error == null && onSuccess != null)
                        onSuccess();

                    if (args.Error != null && onFail != null)
                    {
                        args.IsErrorHandled = true;
                        onFail(args.Error);
                    }
                });
            return op.AsOperationResult();
        }

#endif

        internal static void EnsureRequiredProbeAssemblies()
        {
            IdeaBladeConfig.Instance.ProbeAssemblyNames.Add(typeof(EntityManagerProviderBase<>).Assembly.FullName);
        }

        internal static IEnumerable<Export> GetExportsCore(Type serviceType, string key, CreationPolicy policy)
        {
            string contractName = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            string requiredTypeIdentity = serviceType != null
                                              ? AttributedModelServices.GetTypeIdentity(serviceType)
                                              : null;
            var importDef = new ContractBasedImportDefinition(
                contractName,
                requiredTypeIdentity,
                Enumerable.Empty<KeyValuePair<string, Type>>(),
                ImportCardinality.ZeroOrMore,
                false,
                true,
                policy);

            return Container.GetExports(importDef);
        }

        internal static bool ExportExists<T>()
        {
            return Container.GetExports<T>().Any();
        }

        internal static bool IsRecomposing { get; set; }

        #region Private Methods

        private static void CheckIfConfigured()
        {
            if (!IsConfigured)
                throw new InvalidOperationException(StringResources.CompositionHelperIsNotConfigured);
        }

        #endregion

        #region DesignTime Functionality

        private static readonly Func<bool> IsInDesignModeDefault = () => Execute.InDesignMode;

        /// <summary>Function to determine if in DesignMode. Can be replaced for testing.</summary>
        /// <value>A delegate returning true if in design mode.</value>
        internal static Func<bool> IsInDesignMode = IsInDesignModeDefault;

        /// <summary>
        /// Restore <see cref="IsInDesignMode"/> to default method. For testing.
        /// </summary>
        internal static void ResetIsInDesignModeToDefault()
        {
            IsInDesignMode = IsInDesignModeDefault;
        }

        #endregion
    }

#if SILVERLIGHT

    internal class XapDownloadOperation : INotifyCompleted
    {
        private readonly DynamicXap _xap;
        private XapDownloadCompletedEventArgs _completedEventArgs;
        private Action<INotifyCompletedArgs> _notifyCompletedActions;

        public XapDownloadOperation(string xapUri)
        {
            _xap = new DynamicXap(new Uri(xapUri, UriKind.Relative));
            _xap.Loaded += (s, args) => XapDownloadCompleted(args);
        }

        private void XapDownloadCompleted(DynamicXapLoadedEventArgs args)
        {
            Exception error = null;
            if (!args.HasError)
            {
                Composition.IsRecomposing = true;
                try
                {
                    CompositionHost.Add(_xap);
                }
                catch (Exception e)
                {
                    error = e;
                }
                finally
                {
                    Composition.IsRecomposing = false;
                }
            }

            _completedEventArgs = new XapDownloadCompletedEventArgs(args.Cancelled, args.Error ?? error);

            CallCompletedActions();
        }

        protected void CallCompletedActions()
        {
            Action<INotifyCompletedArgs> actions = _notifyCompletedActions;
            _notifyCompletedActions = null;
            if (actions == null) return;
            actions(_completedEventArgs);
        }

        #region Implementation of INotifyCompleted

        /// <summary>
        /// Action to be performed when the asynchronous operation completes.
        /// </summary>
        /// <param name="completedAction"/>
        public void WhenCompleted(Action<INotifyCompletedArgs> completedAction)
        {
            if (completedAction == null) return;
            if (_completedEventArgs != null)
            {
                completedAction(_completedEventArgs);
                return;
            }
            _notifyCompletedActions =
                (Action<INotifyCompletedArgs>)Delegate.Combine(_notifyCompletedActions, completedAction);
        }

        /// <summary>
        /// Returns whether the operation completed successfully
        /// </summary>
        public bool CompletedSuccessfully
        {
            get { return _completedEventArgs != null && !_completedEventArgs.HasError; }
        }

        /// <summary>
        /// Returns whether the operation failed.
        /// </summary>
        public bool HasError
        {
            get { return _completedEventArgs != null && _completedEventArgs.HasError; }
        }

        /// <summary>
        /// The exception if the action failed.
        /// </summary>
        public Exception Error
        {
            get { return _completedEventArgs != null ? _completedEventArgs.Error : null; }
        }

        #endregion
    }

    internal class XapDownloadCompletedEventArgs : EventArgs, INotifyCompletedArgs
    {
        private readonly bool _cancelled;
        private readonly Exception _error;
        //private readonly DynamicXapLoadedEventArgs _dynamicXapLoadedEventArgs;

        public XapDownloadCompletedEventArgs(bool cancelled, Exception error)
        {
            _cancelled = cancelled;
            _error = error;
            //_dynamicXapLoadedEventArgs = dynamicXapLoadedEventArgs;
        }

        #region Implementation of INotifyCompletedArgs

        /// <summary>
        /// The exception if the action failed.
        /// </summary>
        public Exception Error
        {
            get { return _error; /*_dynamicXapLoadedEventArgs.Error;*/ }
        }

        /// <summary>
        /// Whether the action was cancelled.
        /// </summary>
        public bool Cancelled
        {
            get { return _cancelled; /*_dynamicXapLoadedEventArgs.Cancelled;*/ }
        }

        /// <summary>
        /// Returns whether the operation failed.
        /// </summary>
        public bool HasError { get { return _error != null; /*_dynamicXapLoadedEventArgs.HasError;*/ } }

        /// <summary>
        /// Whether the error was handled.
        /// </summary>
        public bool IsErrorHandled { get; set; }

        #endregion
    }

#endif
}