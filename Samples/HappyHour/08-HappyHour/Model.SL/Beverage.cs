using System;

namespace Model
{
    public class Beverage
    {
        protected static int NextId = 1;

        public Beverage()
        {
            Id = NextId++;
            BeverageName = "<new Beverage>";
            Created = DateTime.UtcNow;
            HasAlcohol = true;
        }
        public int Id { get; protected set; }
        public string BeverageName { get; set; }
        public DateTime Created { get; protected set; }
        public string ImageFilename { get; set; }
        public bool HasAlcohol { get; set; }

        public override string ToString()
        {
            return "Beverage: " + Id + " " + BeverageName;
        }

        /// <summary>The designated "nullo" entity.</summary>
        public static Beverage NullDefaultBeverage =
            new Beverage
                {
                    BeverageName = "<no beverage specified>",
                    HasAlcohol = false,
                };
    }
}