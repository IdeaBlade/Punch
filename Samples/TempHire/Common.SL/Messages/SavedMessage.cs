using System.Collections.Generic;

namespace Common.Messages
{
    public class SavedMessage
    {
        public SavedMessage(IList<object> entities)
        {
            Entities = entities;
        }

        public IList<object> Entities { get; private set; }
    }
}