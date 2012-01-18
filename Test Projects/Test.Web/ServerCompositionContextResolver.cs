using System.Collections.Generic;
using IdeaBlade.Core.Composition;

namespace Test.Web
{
    public class ServerCompositionContextResolver : ICompositionContextResolver
    {
        private static readonly Dictionary<string, CompositionContext> CompositionContexts =
            new Dictionary<string, CompositionContext>();

        #region ICompositionContextResolver Members

        public CompositionContext GetCompositionContext(string compositionContextName)
        {
            if (compositionContextName == CompositionContext.Default.Name ||
                compositionContextName == CompositionContext.Fake.Name)
                return null;

            if (!CompositionContexts.ContainsKey(compositionContextName))
                CompositionContexts.Add(compositionContextName, CompositionContext.Fake.WithName(compositionContextName));

            return CompositionContexts[compositionContextName];
        }

        #endregion
    }
}