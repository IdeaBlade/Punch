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

using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>Interface for a SampleDataProvider. 
    /// SampleDataProviders are used during design time and by FakeStoreEntityManagerProviders
    /// to provide sample data.
    /// Multiple SampleDataProviders are supported. The Framework will discover all providers
    /// and combine the sample data.
    /// Each SampleDataProvider must be exported in order to be discovered by the framework.</summary>
    /// <typeparam name="T">The type of EntityManager supported by this SampleDataProvider</typeparam>
    /// <example>
    /// 	<code title="Static export of a SampleDataProvider" description="Illustrates how to statically export a SampleDataProvider" lang="CS">
    ///     [Export(typeof(ISampleDataProvider&lt;NorthwindIBEntities&gt;))]
    ///     public class SampleDataProvider : ISampleDataProvider&lt;NorthwindIBEntities&gt;
    ///     {
    ///         #region ISampleDataProvider&lt;NorthwindIBEntities&gt; Members
    ///  
    ///         void ISampleDataProvider&lt;NorthwindIBEntities&gt;.AddSampleData(NorthwindIBEntities manager)
    ///         {
    ///             ...
    ///         }
    ///  
    ///         #endregion
    ///     }</code>
    /// 	<code title="Dynamic export of SampleDataProvider at runtime" description="Illustrates how to programatically inject a specific SampleDataProvider at runtime." lang="CS">
    ///     public class SampleDataProvider : ISampleDataProvider&lt;NorthwindIBEntities&gt;
    ///     {
    ///         #region ISampleDataProvider&lt;NorthwindIBEntities&gt; Members
    ///  
    ///         void ISampleDataProvider&lt;NorthwindIBEntities&gt;.AddSampleData(NorthwindIBEntities manager)
    ///         {
    ///             ...
    ///         }
    ///  
    ///         #endregion
    ///     }
    ///  
    ///     // Typically performed in Application Bootstrapper
    ///     var batch = new CompositionBatch();
    ///     batch.AddExportedValue&lt;ISampleDataProvider&lt;NorthwindIBEntities&gt;&gt;(new SampleDataProvider());
    ///     Composition.Configure(batch);
    /// </code>
    /// </example>
    public interface ISampleDataProvider<in T>
        where T : EntityManager
    {
        /// <summary>This method is called by the EntityManagerProvider to initialize the EntityManager cache with sample data. Use EntityManager.AttachEntities or
        /// EntityManager.CacheStateManager.RestoreCacheState() to populate the cache with sample data. The latter is useful to popluate the EntityManager with previously
        /// saved data.</summary>
        /// <param name="manager">The EntityManager that needs to be populated with the sample data.</param>
        void AddSampleData(T manager);
    }
}