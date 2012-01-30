using System.ComponentModel.Composition.Hosting;
using IdeaBlade.TestFramework;

namespace Cocktail.Tests.Helpers
{
    public class CocktailTestBase : DevForceTest
    {
        /// <summary>
        /// Called before each test
        /// </summary>
        protected override void Context()
        {
            Composition.Clear();
            var batch = new CompositionBatch();
            PrepareCompositionContainer(batch);
            Composition.Configure(batch);
        }

        protected virtual void PrepareCompositionContainer(CompositionBatch batch)
        {
            
        }
    }
}