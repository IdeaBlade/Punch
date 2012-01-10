using IdeaBlade.EntityModel;

namespace Security
{
    public class SecurityEntities : EntityManager
    {
        public SecurityEntities(bool shouldConnect = true, string compositionContextName = null)
            : base(shouldConnect, compositionContextName: compositionContextName)
        {
        }

        public SecurityEntities(EntityManager entityManager) : base(entityManager)
        {
        }

        public EntityQuery<User> Users { get; set; }
    }
}