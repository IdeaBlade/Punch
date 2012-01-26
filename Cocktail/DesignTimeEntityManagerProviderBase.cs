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
    public abstract class DesignTimeEntityManagerProviderBase<T> : EntityManagerProviderBase<T>
        where T : EntityManager
    {
        /// <summary>Initializes a new instance.</summary>
        /// <param name="sampleDataProviders">An optional collection of sample data providers. If not provided, the framework will attempt to discover sample data providers.</param>
        protected DesignTimeEntityManagerProviderBase(params ISampleDataProvider<T>[] sampleDataProviders)
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