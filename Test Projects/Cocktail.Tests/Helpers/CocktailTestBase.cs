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
            TestEventAggregator.Reset();
            Composition.ResetIsInDesignModeToDefault();
            Composition.Configure();
        }
    }
}