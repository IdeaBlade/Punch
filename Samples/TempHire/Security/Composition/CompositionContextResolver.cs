using IdeaBlade.Core.Composition;

namespace Security.Composition
{
    public class CompositionContextResolver : ICompositionContextResolver
    {
        private const string TempHireContextName = "TempHireFake";

        public static CompositionContext TempHireFake = CompositionContext.Fake
#if !SILVERLIGHT
            .WithGenerator(typeof(FakeLoginManager))
#endif
            .WithName(TempHireContextName);

        #region ICompositionContextResolver Members

        public CompositionContext GetCompositionContext(string compositionContextName)
        {
            switch (compositionContextName)
            {
                case TempHireContextName:
                    return TempHireFake;
                default:
                    return null;
            }
        }

        #endregion
    }
}