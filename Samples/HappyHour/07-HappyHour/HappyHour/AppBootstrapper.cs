using Cocktail;

namespace HappyHour
{
    public class AppBootstrapper : FrameworkBootstrapper<ViewModels.MainPageViewModel>
    {      
        // Needed if the ListBox displays DrinkOrderEntityViews
        protected override void Configure()
        {
            base.Configure();
            Caliburn.Micro.ViewLocator.NameTransformer
                // Map "Model" classes to views in "HappyHour.Views"
                .AddRule(@"^Model\.(\w*)(?<basename>)", "HappyHour.Views.${1}Entity${basename}View"); 
            ConfigurePathToImageSourceConverter();
        }

        private static void ConfigurePathToImageSourceConverter()
        {
            const string basepath = "/HappyHour;component/assets/images/";

            PathToImageSourceConverter.DefaultPathFilter =
                path => string.IsNullOrEmpty(path) ? path : basepath + path;

            PathToImageSourceConverter.DefaultMissingImage =
                PathToImageSourceConverter.GetImageFromPath(
                basepath + "missing_drink.jpg");
        }
    }
}
