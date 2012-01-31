namespace Model
{
    public static class DrinkImageFilenames
    {
        public static string GetNameById(int id)
        {
            return ImageFilenames[id % ImageFilenames.Length];

            #region The evil base-path approach

            var filename = ImageFilenames[id%ImageFilenames.Length];
            if (string.IsNullOrEmpty(filename)) return filename;

            // return base path + filename
            // ToDo: Get rid of this base path!
            return "/HappyHour;component/assets/images/" + filename;

            #endregion
        }

        private static readonly string[] ImageFilenames = new []
            {
                null, // drink with no image filename
                string.Empty, // drink with empty image file name
                "01_manhattan.jpg",
                "02_blue_martini.jpg",
                "badname.xxx", // image name that doesn't exist
                "03_blue_orange.jpg",
                "04_blue_twist.jpg",
                "05_cinnamon.jpg",
                "06_cosmo.jpg",
                "07_cranberry.jpg",
                "08_cuba_libre.jpg",
                "09_green_twist.jpg",
                "10_martini.jpg",
                "11_mojito.jpg",
                "12_red_tropical.jpg",
                "13_orange_juice.jpg",
                "14_pineapple_juice.jpg",
                "15_cointreau_blush.jpg",
                "16_apple-juice.jpg",
                "17_grapefruit-juice.jpg",
                "18_bloody_mary.jpg",
                "19_margarita.jpg",
                "20_champagne.jpg",
                "21_kir_royale.jpg",
                "22_screaming_orgasm.jpg",
                "23_B52.jpg",
                "24_pina-colada.jpg",
                "25_strawberry_daiquiri.jpg",
                "26_mango_daiquiri.jpg",
                "27_tequila_sunrise.jpg",
                "28_screwdriver.jpg",
                "29_long_island_iced_tea.jpg",
                "30_chi_chi.jpg",
                "32_blue_lagoon.jpg",
                "33_old_fashioned.jpg",
                "34_cola.jpg",
                "35_gin_and_tonic.jpg",
                "36_midori-rickey.jpg",
                "37_scotch_and_soda.jpg",
            };
    }
}