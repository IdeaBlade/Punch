using System.Collections.Generic;
using IdeaBlade.Core.Composition;

namespace Cocktail.Tests.Helpers
{
    public class CompositionContextResolver : ICompositionContextResolver
    {
        private static readonly Dictionary<string, CompositionContext> CompositionContexts =
            new Dictionary<string, CompositionContext>();

        public CompositionContext GetCompositionContext(string compositionContextName)
        {
            if (!CompositionContexts.ContainsKey(compositionContextName)) 
                return null;

            return CompositionContexts[compositionContextName];
        }

        public static void Add(CompositionContext compositionContext)
        {
            if (CompositionContexts.ContainsKey(compositionContext.Name))
                CompositionContexts.Remove(compositionContext.Name);

            CompositionContexts.Add(compositionContext.Name, compositionContext);
        }
    }
}