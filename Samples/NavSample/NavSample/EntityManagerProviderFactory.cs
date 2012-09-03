using System.Composition;
using Cocktail;

namespace NavSample
{
    public class EntityManagerProviderFactory
    {
        [Export]
        public IEntityManagerProvider<NorthwindIBEntities> EntityManagerProvider
        {
            get
            {
                return new EntityManagerProvider<NorthwindIBEntities>();
            }
        }
    }
}