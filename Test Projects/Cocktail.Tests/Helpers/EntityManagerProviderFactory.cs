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

using System.ComponentModel.Composition;
using Test.Model;

namespace Cocktail.Tests.Helpers
{
    public class EntityManagerProviderFactory
    {
        public static IEntityManagerProvider<NorthwindIBEntities> CreateTestEntityManagerProvider(
            string compositionContextName = null)
        {
            if (string.IsNullOrEmpty(compositionContextName))
                return new EntityManagerProvider<NorthwindIBEntities>().With(ConnectionOptions.Fake.Name);

            ConnectionOptions connectionOptions =
                ConnectionOptions.Default.WithCompositionContext(compositionContextName).WithName(compositionContextName);
            ConnectionOptionsResolver.Add(connectionOptions);
            return new EntityManagerProvider<NorthwindIBEntities>().With(compositionContextName);
        }

        [Export]
        public IEntityManagerProvider<NorthwindIBEntities>  EntityManagerProvider
        {
            get { return CreateTestEntityManagerProvider(); }
        }
    }
}