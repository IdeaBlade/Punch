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