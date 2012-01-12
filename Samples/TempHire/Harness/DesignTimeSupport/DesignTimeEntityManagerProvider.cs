using Cocktail;
using Common.SampleData;
using DomainModel;

namespace TempHire.DesignTimeSupport
{
    public class DesignTimeEntityManagerProvider : DesignTimeEntityManagerProviderBase<TempHireEntities>
    {
        public DesignTimeEntityManagerProvider(TempHireSampleDataProvider[] sampleDataProviders)
            : base(sampleDataProviders)
        {
        }

        protected override TempHireEntities CreateEntityManager()
        {
            return new TempHireEntities(false);
        }
    }
}