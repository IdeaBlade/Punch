//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;

namespace IdeaBlade.Application.Framework.Core.Composition
{
    /// <summary>Implementation of the global Composition Catalog Service. The service is available as a shared instance through a MEF export and can be
    /// imported to any class that needs to query or modify the Composition Catalog.</summary>
    /// <example>
    /// 	<code title="Example" description="Shows how to inject the Composition Service." lang="CS">
    /// [Export]
    /// public class UseCompositonCatalog
    /// {
    ///     private readonly ICompositionCatalogService _catalogService;
    ///  
    ///     [ImportingConstructor]
    ///     public UseCompositionCatalog(ICompositionCatalogService catalogService)
    ///     {
    ///         _catalogService = catalogService;
    ///     }
    /// }</code>
    /// </example>
    [Export(typeof(ICompositionCatalogService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class CompositionCatalogService : ICompositionCatalogService
    {
        #region Implementation of ICompositionCatalogService

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instance.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public T GetInstance<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return CompositionHelper.GetInstance<T>(requiredCreationPolicy);
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instances.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public IEnumerable<T> GetInstances<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return CompositionHelper.GetInstances<T>(requiredCreationPolicy);
        }

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type or contract name.</para>
        /// </summary>
        /// <param name="serviceType">The type of the requested instance.</param>
        /// <param name="key">The contract name of the instance requested. If no contract name is specifed, the type will be used.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public object GetInstance(Type serviceType, string key, CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return CompositionHelper.GetInstance(serviceType, key, requiredCreationPolicy);
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <param name="serviceType">Type of the requested instances.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public IEnumerable<object> GetInstances(Type serviceType, CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return CompositionHelper.GetInstances(serviceType, requiredCreationPolicy);
        }

        /// <summary>Returns an instance of the custom implementation for the provided type. If no custom implementation is found, an instance of the default
        /// implementation is returned.</summary>
        /// <param name="serviceType">The type for which an instance is being requested.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public object GetCustomInstanceOrDefault(Type serviceType, CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return CompositionHelper.GetCustomInstanceOrDefault(serviceType, requiredCreationPolicy);
        }

#if SILVERLIGHT

        private readonly Dictionary<string, XapDownloadOperation> _xapDownloadOperations;

        /// <summary>
        /// Creates and initializes a new CompositionCatalogService
        /// </summary>
        public CompositionCatalogService()
        {
            _xapDownloadOperations = new Dictionary<string, XapDownloadOperation>();
        }

        /// <summary>Asynchronously downloads a XAP file and adds all exported parts to the catalog in use.</summary>
        /// <param name="relativeUri">The relative URI for the XAP file to be downloaded.</param>
        /// <param name="onSuccess">User callback to be called when operation completes successfully.</param>
        /// <param name="onFail">User callback to be called when operation completes with an error.</param>
        /// <returns>Returns a handle to the download operation.</returns>
        /// <example>
        /// 	<code title="Example" description="Demonstrates how to load a XAP file by using a Coroutine before querying the CompositionCatalogService." source="..\..\Workspace\IdeaBlade\IdeaBlade.Application.Framework\Samples\HelloWorld\HelloWorld\EditOrderMessageProcessor.cs" lang="CS"></code>
        /// </example>
        public INotifyCompleted AddXap(string relativeUri, Action onSuccess = null, Action<Exception> onFail = null)
        {
            XapDownloadOperation operation;
            if (_xapDownloadOperations.TryGetValue(relativeUri, out operation) && !operation.HasError) return operation;

            var op = _xapDownloadOperations[relativeUri] = new XapDownloadOperation(relativeUri);
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
            return op;
        }

#endif

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
                CompositionHelper.IsRecomposing = true;
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
                    CompositionHelper.IsRecomposing = false;
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