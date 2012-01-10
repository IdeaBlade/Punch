using IdeaBlade.Application.Framework.Core.Persistence;

namespace DomainModel
{
    public class TempHireEntityManagerProvider : BaseEntityManagerProvider<TempHireEntities>
    {
        protected override TempHireEntities CreateEntityManager()
        {
            return new TempHireEntities();
        }
    }
}