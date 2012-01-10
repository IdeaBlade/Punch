//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
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
using System.ComponentModel.Composition.Hosting;

namespace Cocktail
{
    /// <summary>The CompositionContext scopes MEF discovery requests and allows the injection of specifc instances.</summary>
    public class CompositionContext
    {
        private readonly Dictionary<Type, object> _instances;
        private CompositionContainer _childContainer;

        private static CompositionContext _default;

        /// <summary>Default IoC container/context used if not otherwise specified.</summary>
        public static CompositionContext Default
        {
            get { return _default ?? (_default = new CompositionContext()); }
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public CompositionContext()
        {
            _instances = new Dictionary<Type, object>();

            //if (CompositionHelper.IsConfigured)
            //    _childContainer = new CompositionContainer(CompositionHelper.Container);
        }

        internal CompositionContainer ChildContainer
        {
            get
            {
                if (!CompositionHelper.IsConfigured) return null;

                if (_childContainer != null)
                    return _childContainer;
                return _childContainer = new CompositionContainer(CompositionHelper.Container);
            }
        }

        /// <summary>Returns an instance of the custom implementation for the provided type within the scope of the context's child container. If no custom
        /// implementation is found, an instance of the default implementation is returned.</summary>
        /// <typeparam name="T">Type of the requested instance.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        /// <seealso cref="AddInstance{T}"></seealso>
        public T GetInstance<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.NonShared)
        {
            // Find the instance associated with this context
            object instance;
            if (_instances.TryGetValue(typeof(T), out instance)) return (T)instance;

            // Get a new instance of T from the MEF container
            instance = CompositionHelper.GetCustomInstanceOrDefault(typeof(T), ChildContainer, requiredCreationPolicy);
            BuildUp(instance);

            return (T)instance;
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type within the scope of the context's child container.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instances.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public IEnumerable<T> GetInstances<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.NonShared)
        {
            return CompositionHelper.GetInstances<T>(ChildContainer, requiredCreationPolicy);
        }

        /// <summary>Injects a specific instance into the context. Future calls to GetInstance&lt;T&gt;() and GetInstances() are guaranteed to return this
        /// instance. If the instance implements IContextAware, the context automatically associates itself.</summary>
        /// <typeparam name="T">The type by which this instance should be referenced. By default, T is the concrete type of the instance. If the instance should be found by one
        /// of its base classes or implemented interfaces instead, specify the base class or interface.</typeparam>
        /// <param name="instance">The instance to be added to the context. Optionally specify T as a base class or interface implemented by the instance, to retrieve the instance
        /// by a type other than the concrete type of the instance.</param>
        /// <seealso cref="GetInstance{T}"></seealso>
        public void AddInstance<T>(T instance)
        {
            if (CompositionHelper.IsConfigured)
                ChildContainer.ComposeExportedValue(instance);
            else
            {
                _instances.Add(typeof(T), instance);
            }
        }

        /// <summary>Satisfies all the imports on the provided instance using the context's child container.</summary>
        /// <param name="instance">The instance for which to satisfy the MEF imports.</param>
        public void BuildUp(object instance)
        {
            CompositionHelper.BuildUp(instance, ChildContainer);
        }
    }
}