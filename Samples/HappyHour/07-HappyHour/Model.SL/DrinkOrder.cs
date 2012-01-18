namespace Model
{
    public class DrinkOrder
    {
        protected static int NextId = 1;

        protected static Beverage DefaultBeverage =
            new Beverage {BeverageName = "<unknown drink>"};

        public DrinkOrder(Beverage beverage)
        {
            Id = NextId++;
            Created = System.DateTime.UtcNow;
            Beverage = beverage;
        }

        internal DrinkOrder() : this(DefaultBeverage) { }

        public int Id { get; protected set; }
        public System.DateTime Created { get; protected set; }
        public Beverage Beverage { get; protected set; }

        // Sugar properties: delegate to Beverage
        public string DrinkName { get { return Beverage.BeverageName; } }
        public string ImageFilename { get { return Beverage.ImageFilename; } }
        public bool HasAlcohol { get { return Beverage.HasAlcohol; } }

        public override string ToString()
        {
            return "DrinkOrder: " + Id + " " + DrinkName;
        }
    }
}
