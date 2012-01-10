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
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    /// Base class for an EntityManagerProvider with sample data
    /// Extend from this class to implement a Design Time EntityManagerProvider
    /// The sample data is provided by means of implementing ISampleDataProvider&lt;in T&gt;
    /// </summary>
    /// <typeparam name="T">The type of the EntityManager</typeparam>    
    public abstract class BaseDesignTimeEntityManagerProvider<T> : BaseEntityManagerProvider<T>
        where T : EntityManager
    {
        /// <summary>Initializes a new instance.</summary>
        /// <param name="sampleDataProviders">An optional collection of sample data providers. If not provided, the framework will attempt to discover sample data providers.</param>
        protected BaseDesignTimeEntityManagerProvider(params ISampleDataProvider<T>[] sampleDataProviders)
        {
            SampleDataProviders = sampleDataProviders;
        }

        /// <summary>Internal use.</summary>
        [ImportMany]
        public IEnumerable<ISampleDataProvider<T>> SampleDataProviders { get; set; }

        /// <summary>Internal use.</summary>
        protected override T CreateEntityManagerCore()
        {
            T manager = base.CreateEntityManagerCore();

            manager.Fetching +=
                delegate { throw new InvalidOperationException(StringResources.ManagerTriedToFetchData); };
            manager.Saving +=
                delegate { throw new InvalidOperationException(StringResources.ManagerTriedToSaveData); };

            if (SampleDataProviders != null)
                SampleDataProviders.ForEach(p => p.AddSampleData(manager));

            return manager;
        }
    }
}