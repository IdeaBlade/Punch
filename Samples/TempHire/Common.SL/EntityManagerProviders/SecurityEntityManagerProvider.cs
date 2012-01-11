using Cocktail;
using Security;

namespace Common.EntityManagerProviders
{
    public class SecurityEntityManagerProvider : BaseEntityManagerProvider<SecurityEntities>
    {
        protected override SecurityEntities CreateEntityManager()
        {
            return new SecurityEntities();
        }
    }
}