namespace Model
{
    public class DrinkOrder
    {
        public DrinkOrder() : this(DefaultBeverage) { }

        public DrinkOrder(Beverage beverage)
        {
            Id = NextId++;
            Created = System.DateTime.UtcNow;
            Beverage = beverage;
        }

        protected static Beverage DefaultBeverage =
            new Beverage { BeverageName = "<unknown drink>" };

        public int Id { get; protected set; }
        public System.DateTime Created { get; protected set; }

        private Beverage _beverage;
        public Beverage Beverage
        {
            get { return _beverage ?? Beverage.NullDefaultBeverage; }
            protected set { _beverage = value; }
        }

        public override string ToString()
        {
            return "DrinkOrder: " + Id + " " + Beverage.BeverageName;
        }

        protected static int NextId = 1;
    }
}
