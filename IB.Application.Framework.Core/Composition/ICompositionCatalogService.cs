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
using IdeaBlade.EntityModel;

namespace IdeaBlade.Application.Framework.Core.Composition
{
    /// <summary>Provides methods to query and modify the MEF container</summary>
    /// <seealso cref="CompositionCatalogService"></seealso>
    public interface ICompositionCatalogService
    {
        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instance.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        T GetInstance<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any);

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instances.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        IEnumerable<T> GetInstances<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any);

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type or contract name.</para>
        /// </summary>
        /// <param name="serviceType">The type of the requested instance.</param>
        /// <param name="key">The contract name of the instance requested. If no contract name is specifed, the type will be used.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        object GetInstance(Type serviceType, string key, CreationPolicy requiredCreationPolicy = CreationPolicy.Any);

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <param name="serviceType">Type of the requested instances.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        IEnumerable<object> GetInstances(Type serviceType, CreationPolicy requiredCreationPolicy = CreationPolicy.Any);

        /// <summary>Returns an instance of the custom implementation for the provided type. If no custom implementation is found, an instance of the default
        /// implementation is returned.</summary>
        /// <param name="serviceType">The type for which an instance is being requested.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        object GetCustomInstanceOrDefault(Type serviceType, CreationPolicy requiredCreationPolicy = CreationPolicy.Any);

#if SILVERLIGHT

        /// <summary>Asynchronously downloads a XAP file and adds all exported parts to the catalog in use.</summary>
        /// <param name="relativeUri">The relative URI for the XAP file to be downloaded.</param>
        /// <param name="onSuccess">User callback to be called when operation completes successfully.</param>
        /// <param name="onFail">User callback to be called when operation completes with an error.</param>
        /// <returns>Returns a handle to the download operation.</returns>
        /// <example>
        /// 	<code title="Example" description="Demonstrates how to load a XAP file by using a Coroutine before querying the CompositionCatalogService." source="..\..\Workspace\IdeaBlade\IdeaBlade.Application.Framework\Samples\HelloWorld\HelloWorld\EditOrderMessageProcessor.cs" lang="CS"></code>
        /// 	<code title="Updating the Caliburn Micro AssemblySource" description="Demonstrates how to update the Caliburn Micro AssemblySource in response to a XAP file being added to the CompositionContainer. This makes sure that Caliburn will find views in the new assemblies." lang="CS">
        /// IdeaBlade.Core.Composition.CompositionHost.Recomposed += RefreshCaliburnAssemblySource;
        ///  
        /// private void RefreshCaliburnAssemblySource(object sender, EventArgs e)
        /// {
        ///     var catalogs = CompositionHelper.Catalog.Catalogs.OfType&lt;AssemblyCatalog&gt;();
        ///     var assemblySource = AssemblySource.Instance;
        ///  
        ///     catalogs.ForEach(
        ///         c =&gt;
        ///             {
        ///                 if (!assemblySource.Contains(c.Assembly))
        ///                     assemblySource.Add(c.Assembly);
        ///             });
        /// }</code>
        /// </example>
        INotifyCompleted AddXap(string relativeUri, Action onSuccess = null, Action<Exception> onFail = null);

#endif
    }
}
