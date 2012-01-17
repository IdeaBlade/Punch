namespace Model
{
    public class DrinkOrder
    {
        protected static int NextId = 1;

        public DrinkOrder()
        {
            Id = NextId++;
            DrinkName = "<new DrinkOrder>";
            Created = System.DateTime.UtcNow;
        }
        public int Id { get; private set; }
        public string DrinkName { get; set; }
        public System.DateTime Created { get; private set; }
    }
}
