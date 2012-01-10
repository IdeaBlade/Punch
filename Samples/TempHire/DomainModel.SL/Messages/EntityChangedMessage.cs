namespace DomainModel.Messages
{
    public class EntityChangedMessage
    {
        public EntityChangedMessage(object entity)
        {
            Entity = entity;
        }

        public object Entity { get; private set; }
    }
}