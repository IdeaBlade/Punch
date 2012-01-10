using IdeaBlade.Application.Framework.Core.Persistence;

namespace Security
{
    public class SecurityEntityManagerProvider : BaseEntityManagerProvider<SecurityEntities>
    {
        protected override SecurityEntities CreateEntityManager()
        {
            return new SecurityEntities();
        }
    }
}