using Cocktail;

namespace HappyHour
{
    public class AppBootstrapper : FrameworkBootstrapper<ViewModels.MainPageViewModel>
    {          
        // Needed if the ListBox displays DrinkOrderEntityViews
        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            Caliburn.Micro.ViewLocator.NameTransformer
                // Map "Model" classes to views in "HappyHour.Views"
                .AddRule(@"^Model\.(\w*)", "HappyHour.Views.${1}EntityView");

            base.OnStartup(sender, e);
        }

        protected override void AddValueConverterConventions()
        {
            const string basepath = "/HappyHour;component/assets/images/";

            PathToImageSourceConverter.DefaultPathFilter =
                path => string.IsNullOrEmpty(path) ? path : basepath + path;

            PathToImageSourceConverter.DefaultMissingImage =
                PathToImageSourceConverter.GetImageFromPath(
                basepath + "missing_drink.jpg");

            base.AddValueConverterConventions();
        }
    }
}
