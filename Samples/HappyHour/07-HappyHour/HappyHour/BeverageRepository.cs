using System.Collections.Generic;
using Model;

namespace HappyHour
{
    public interface IBeverageRepository
    {
        IEnumerable<Beverage> FindAll();

        // Should have an async signature. We learn about this in a latter tutorial
        //void FindAll(Action<IEnumerable<Beverage>> onSuccess, Action<Exception> onFail);
    }

    public class BeverageRepository : IBeverageRepository
    {
        private IEnumerable<Beverage> Beverages { get; set; }

        public BeverageRepository()
        {
            Beverages = GetFakedBeverageInventory(); 
        }

        // Should have an async signature. We learn about this in a latter tutorial
        //public void FindAll(Action<IEnumerable<Beverage>> onSuccess, Action<Exception> onFail) {}

        public IEnumerable<Beverage> FindAll()
        {
            return Beverages; /* Would get from remote store */
        }

        #region GetFakedBeverageInventory 

        private IEnumerable<Beverage> GetFakedBeverageInventory()
        {
            return new []
                       {
                           new Beverage {BeverageName = "Manhattan", ImageFilename = "01_manhattan.jpg"},
                           new Beverage {BeverageName = "Blue Martini", ImageFilename = "02_blue_martini.jpg"},
                           new Beverage {BeverageName = "Blue Orange", ImageFilename = "03_blue_orange.jpg"},
                           new Beverage {BeverageName = "Blue Twist", ImageFilename = "04_blue_twist.jpg"},
                           new Beverage {BeverageName = "Cinnamon", ImageFilename = "05_cinnamon.jpg"},
                           new Beverage {BeverageName = "Rum and Coke", ImageFilename = null}, // null image name
                           new Beverage {BeverageName = "Cosmo", ImageFilename = "06_cosmo.jpg"},
                           new Beverage {BeverageName = "Cranberry Juice",ImageFilename = "07_cranberry.jpg",HasAlcohol = false},
                           new Beverage {BeverageName = "Whiskey Sour", ImageFilename = string.Empty}, // blank image name
                           new Beverage {BeverageName = "Cuba Libre", ImageFilename = "08_cuba_libre.jpg"},
                           new Beverage {BeverageName = "Green Twist", ImageFilename = "09_green_twist.jpg"},
                           new Beverage {BeverageName = "Harvey Wallbanger", ImageFilename = "badname.xxx"}, // bad image name
                           new Beverage {BeverageName = "Martini", ImageFilename = "10_martini.jpg"},
                           new Beverage {BeverageName = "Mojito", ImageFilename = "11_mojito.jpg"},
                           new Beverage {BeverageName = "Cape Codder", ImageFilename = "12_red_tropical.jpg"},
                           new Beverage {BeverageName = "Orange Juice",ImageFilename = "13_orange_juice.jpg",HasAlcohol = false},
                           new Beverage {BeverageName = "Pineapple Juice", ImageFilename = "14_pineapple_juice.jpg", HasAlcohol = false},
                           new Beverage {BeverageName = "Cointreau Blush", ImageFilename = "15_cointreau_blush.jpg"},
                           new Beverage {BeverageName = "Apple Juice", ImageFilename = "16_apple-juice.jpg", HasAlcohol = false}, 
                           new Beverage {BeverageName = "Grapefruit Juice", ImageFilename = "17_grapefruit-juice.jpg", HasAlcohol = false},
                           new Beverage {BeverageName = "Grey Hound", ImageFilename = "17_grapefruit-juice.jpg", HasAlcohol = true},
                           new Beverage {BeverageName = "Bloody Mary", ImageFilename = "18_bloody_mary.jpg"},
                           new Beverage {BeverageName = "Margarita", ImageFilename = "19_margarita.jpg"},
                           new Beverage {BeverageName = "Champagne", ImageFilename = "20_champagne.jpg"},
                           new Beverage {BeverageName = "Kir Royale", ImageFilename = "21_kir_royale.jpg"},
                           new Beverage {BeverageName = "Screaming Orgasm", ImageFilename = "22_screaming_orgasm.jpg"},
                           new Beverage {BeverageName = "B52", ImageFilename = "23_B52.jpg"},
                           new Beverage {BeverageName = "Pina Colada", ImageFilename = "24_pina-colada.jpg"},
                           new Beverage {BeverageName = "Strawberry Daiquiri", ImageFilename = "25_strawberry_daiquiri.jpg"},
                           new Beverage {BeverageName = "Mango Daiquiri", ImageFilename = "26_mango_daiquiri.jpg"},
                           new Beverage {BeverageName = "Tequila Sunrise", ImageFilename = "27_tequila_sunrise.jpg"},
                           new Beverage {BeverageName = "Screwdriver", ImageFilename = "28_screwdriver.jpg"},
                           new Beverage {BeverageName = "Long Island Iced Tea", ImageFilename = "29_long_island_iced_tea.jpg"},
                           new Beverage {BeverageName = "Chi Chi", ImageFilename = "30_chi_chi.jpg"},
                           new Beverage {BeverageName = "Blue Lagoon", ImageFilename = "32_blue_lagoon.jpg"},
                           new Beverage {BeverageName = "Old Fashioned", ImageFilename = "33_old_fashioned.jpg"},
                           new Beverage {BeverageName = "Cola", ImageFilename = "34_cola.jpg", HasAlcohol = false},
                           new Beverage {BeverageName = "Gin and Tonic", ImageFilename = "35_gin_and_tonic.jpg"},
                           new Beverage {BeverageName = "Midori Rickey", ImageFilename = "36_midori-rickey.jpg"},
                           new Beverage {BeverageName = "Scotch and Soda", ImageFilename = "37_scotch_and_soda.jpg"},
                       };
        }

        #endregion
    }
}
