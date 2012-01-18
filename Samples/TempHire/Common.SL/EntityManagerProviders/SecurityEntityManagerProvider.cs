using Cocktail;
using Security;

namespace Common.EntityManagerProviders
{
    public class SecurityEntityManagerProvider : EntityManagerProviderBase<SecurityEntities>
    {
        protected override SecurityEntities CreateEntityManager()
        {
            return new SecurityEntities();
        }
    }
}