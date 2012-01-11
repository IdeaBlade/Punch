using Cocktail;
using DomainModel;

namespace TempHire.DesignTimeSupport
{
    public class DesignTimeEntityManagerProvider : DesignTimeEntityManagerProviderBase<TempHireEntities>
    {
        public DesignTimeEntityManagerProvider(ISampleDataProvider<TempHireEntities>[] sampleDataProviders)
            : base(sampleDataProviders)
        {
        }

        protected override TempHireEntities CreateEntityManager()
        {
            return new TempHireEntities(false);
        }
    }
}