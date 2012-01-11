using System.ComponentModel.Composition;
using Cocktail;
using Common.EntityManagerProviders;
using DomainModel;
using Security;
using TempHire.EntityManagerProviders;

namespace TempHire
{
    public class EntityManagerProviderFactory
    {
        [Export]
        public IEntityManagerProvider<TempHireEntities> TempHireEntityManagerProvider
        {
            get
            {
#if FAKESTORE
                return new DevTempHireEntityManagerProvider();
#else
                return new TempHireEntityManagerProvider();
#endif
            }
        }

        [Export]
        public IEntityManagerProvider<SecurityEntities> SecurityEntityManagerProvider
        {
            get
            {
#if FAKESTORE
                return new DevSecurityEntityManagerProvider();
#else
                return new SecurityEntityManagerProvider();
#endif
            }
        }
    }
}