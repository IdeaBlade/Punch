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

using IdeaBlade.Core.Composition;
using Test.Model;

namespace Cocktail.Tests.Helpers
{
    /// <summary>
    /// Concrete EntityManagerProvider used during development. Uses a fake datastore.
    /// </summary>
    public class DevelopmentEntityManagerProvider : FakeStoreEntityManagerProviderBase<NorthwindIBEntities>
    {
        private readonly string _compositionContextName;

        public DevelopmentEntityManagerProvider(string compositionContextName = null)
        {
            _compositionContextName = compositionContextName ?? CompositionContext.Fake.Name;
        }

        protected override NorthwindIBEntities CreateEntityManager()
        {
            return new NorthwindIBEntities(compositionContextName: _compositionContextName);
        }
    }
}